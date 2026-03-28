
using CRM.Base.Enums;

namespace CRM.Base.Common.Repositories.Implementations;

public abstract class MSSQLBaseRepository<T, I> : IMSSQLRepository<T, I>
    where T : BaseEntity<I>
{
    private readonly IApplicationDbContext _context;

    protected MSSQLBaseRepository(IApplicationDbContext context) => _context = context ?? throw new ArgumentNullException(nameof(context));

    #region CRUD Operations

    public virtual async Task<IQueryable<T>> GetAllAsync()
    {
        return await Task.FromResult(_context.Set<T>());
    }

    public async Task<IList<T>> GetAllAsync(
           string? search = null,
           string? filter = null,
           int page = 1,
           int pageSize = 10,
           string? orderBy = null,
           OrderDirectionEnum orderDirection = OrderDirectionEnum.Asc,
           string baseUrl = "{app_url}")
    {
        // Return only the data (backward compatible)
        var paged = await GetAllWithMetaAsync(
            search, filter, page, pageSize, orderBy, orderDirection,
            baseUrl: baseUrl);

        return paged.Content.ToList();
    }

    public async Task<Result<List<T>>> GetAllWithMetaAsync(
        string? search = null,
        string? filter = null,
        int page = 1,
        int pageSize = 10,
        string? orderBy = null,
        OrderDirectionEnum orderDirection = OrderDirectionEnum.Asc,
        string baseUrl = "{app_url}")
    {
        //if (pageSize <= 0) pageSize = 10;
        //if (page <= 0) page = 1;

        IQueryable<T> query = _context.Set<T>();

        // FILTER
        if (!string.IsNullOrEmpty(filter))
        {
            try
            {
                var filterParts = filter.Split('=', StringSplitOptions.TrimEntries);
                if (filterParts.Length == 2)
                {
                    var propertyName = filterParts[0];
                    var propertyValue = filterParts[1];

                    var parameter = Expression.Parameter(typeof(T), nameof(T));
                    
                    // Allow case-insensitive property resolution
                    var propInfo = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (propInfo != null)
                    {
                        var property = Expression.Property(parameter, propInfo);

                        Expression constant;
                        Expression equalsExpression;

                        if (propInfo.PropertyType == typeof(bool) && bool.TryParse(propertyValue, out var boolValue))
                        {
                            constant = Expression.Constant(boolValue, typeof(bool));
                            equalsExpression = Expression.Equal(property, constant);
                        }
                        else if (propInfo.PropertyType == typeof(DateTimeOffset) && DateTimeOffset.TryParse(propertyValue, out var dto))
                        {
                            constant = Expression.Constant(dto, typeof(DateTimeOffset));
                            equalsExpression = Expression.Equal(property, constant);
                        }
                        else if (propInfo.PropertyType.IsEnum)
                        {
                            var enumValue = Enum.Parse(propInfo.PropertyType, propertyValue, true);
                            constant = Expression.Constant(enumValue, propInfo.PropertyType);
                            // Enum values are technically integers, ensuring strong comparison:
                            equalsExpression = Expression.Equal(Expression.Convert(property, propInfo.PropertyType), constant);
                        }
                        else
                        {
                            constant = Expression.Constant(propertyValue, typeof(string));
                            equalsExpression = Expression.Equal(property, constant);
                        }

                        var lambda = Expression.Lambda<Func<T, bool>>(equalsExpression, parameter);
                        query = query.Where(lambda);
                    }
                }
            }
            // Explicitly swallow invalid filters gracefully without crashing
            catch (Exception ex)
            {
                Console.WriteLine($"Filter skipped due to error: {ex.Message}");
            }
        }

        // ORDER
        if (!string.IsNullOrEmpty(orderBy))
        {
            var prop = typeof(T).GetProperty(orderBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (prop != null)
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                var propertyAccess = Expression.Property(parameter, prop);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                var methodName = orderDirection == OrderDirectionEnum.Desc ? "OrderByDescending" : "OrderBy";

                var resultExp = Expression.Call(
                    typeof(Queryable), methodName,
                    new Type[] { typeof(T), prop.PropertyType },
                    query.Expression, Expression.Quote(orderByExp));

                query = query.Provider.CreateQuery<T>(resultExp);
            }
        }

        // INCLUDE nav props (up to 2 levels deep)
        var navigationProperties = typeof(T).GetProperties()
            .Where(p => typeof(IEnumerable<object>).IsAssignableFrom(p.PropertyType) ||
                       (p.PropertyType.IsClass && p.PropertyType != typeof(string)));

        foreach (var nav in navigationProperties)
        {
            query = query.Include(nav.Name);

            var navType = nav.PropertyType.IsGenericType
                ? nav.PropertyType.GetGenericArguments().FirstOrDefault()
                : nav.PropertyType;

            if (navType != null && navType != typeof(string))
            {
                var childNavs = navType.GetProperties()
                    .Where(p => (typeof(IEnumerable<object>).IsAssignableFrom(p.PropertyType) ||
                                (p.PropertyType.IsClass && p.PropertyType != typeof(string)))
                                && p.PropertyType != typeof(T)); // Avoid circular includes back to parent

                foreach (var childNav in childNavs)
                    query = query.Include($"{nav.Name}.{childNav.Name}");
            }
        }

        // Total BEFORE pagination (note: search is applied in-memory below)
        var totalCount = await query.CountAsync();

        // PAGE BOUNDS
        var lastPage = Math.Max(1, (int)Math.Ceiling((double)totalCount / pageSize));
        if (page > lastPage) page = lastPage;

        var skip = (page - 1) * pageSize;
        var from = totalCount == 0 ? 0 : skip + 1;
        var to = Math.Min(skip + pageSize, totalCount);

        // PAGE DATA
        var results = (page == 0 && pageSize == 0) ? await query.ToListAsync() : await query.Skip(skip).Take(pageSize).ToListAsync();

        // SEARCH (in-memory; if you want DB-side search, we can push it into the expression tree)
        if (!string.IsNullOrEmpty(search))
        {
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            results = results
                .Where(item => props.Any(prop =>
                {
                    var value = prop.GetValue(item)?.ToString();
                    return value != null && value.Contains(search, StringComparison.OrdinalIgnoreCase);
                }))
                .ToList();
            // 'totalCount' doesn't reflect search when search is in-memory.
            // If you need 'Total' to include search, we should move search into the IQueryable.
        }

        // BUILD META (normalize page query to lowercase and no spaces)
        string Q(int n) => $"{baseUrl}?page={n}";

        var result = new Result<List<T>>
        {
            Content = results,
            MetaData = new MetaData
            {
                From = from,
                LastPage = lastPage,
                FirstPageUrl = Q(1),
                LastPageUrl = Q(lastPage),
                NextPageUrl = page < lastPage ? Q(page + 1) : null,
                Path = baseUrl,
                PerPage = pageSize,
                PrevPageUrl = page > 1 ? Q(page - 1) : null,
                To = to,
                Total = totalCount
            }
        };

        return result;
    }



    public virtual async Task<IList<T>> GetAllAsync(Expression<Func<T, bool>> expression)
    {
        var query = _context.Set<T>().Where(expression);

        // Dynamically include all navigation properties
        var navigationProperties = typeof(T).GetProperties()
            .Where(prop => typeof(IEnumerable<object>).IsAssignableFrom(prop.PropertyType) ||
                           (prop.PropertyType.IsClass && prop.PropertyType != typeof(string)));

        foreach (var navigationProperty in navigationProperties)
        {
            query = query.Include(navigationProperty.Name);
        }

        return await query.ToListAsync();
    }

    public virtual async Task<T> GetAsync(Expression<Func<T, bool>> expression)
    {
        return await _context.Set<T>().FirstOrDefaultAsync(expression);
    }

    public virtual T Get(Expression<Func<T, bool>> expression)
    {
        return _context.Set<T>().FirstOrDefault(expression);
    }

    //public virtual async Task<T?> GetByIdAsync(I id)
    //{
    //    ArgumentValidatorHelpers.ValidateArgument(id, nameof(id));
    //    var response = await _context.Set<T>().FirstOrDefaultAsync(x => x.Id.Equals(id));
    //    return response;
    //}

    public virtual async Task<T?> GetByIdAsync(I id)
    {
        ArgumentValidatorHelpers.ValidateArgument(id, nameof(id));

        // Start query
        IQueryable<T> query = _context.Set<T>();

        // Dynamically include all navigation properties (up to 2 levels deep)
        var navigationProperties = typeof(T).GetProperties()
            .Where(prop =>
                typeof(IEnumerable<object>).IsAssignableFrom(prop.PropertyType) ||
                (prop.PropertyType.IsClass && prop.PropertyType != typeof(string)));

        foreach (var nav in navigationProperties)
        {
            query = query.Include(nav.Name);

            var navType = nav.PropertyType.IsGenericType
                ? nav.PropertyType.GetGenericArguments().FirstOrDefault()
                : nav.PropertyType;

            if (navType != null && navType != typeof(string))
            {
                var childNavs = navType.GetProperties()
                    .Where(p => (typeof(IEnumerable<object>).IsAssignableFrom(p.PropertyType) ||
                                (p.PropertyType.IsClass && p.PropertyType != typeof(string)))
                                && p.PropertyType != typeof(T)); // Avoid circular includes back to parent

                foreach (var childNav in childNavs)
                    query = query.Include($"{nav.Name}.{childNav.Name}");
            }
        }

        // Apply ID filter
        var response = await query.FirstOrDefaultAsync(x => x.Id.Equals(id));

        return response;
    }


    public virtual async Task<T?> GetByCodeAsync(string code)
    {
        ArgumentValidatorHelpers.ValidateStringArgument(code, nameof(code));
        return await _context.Set<T>().FindAsync(code);
    }

    public virtual async Task<T> CreateAsync(T entity)
    {
        ArgumentValidatorHelpers.ValidateArgument(entity, nameof(entity));
        await _context.Set<T>().AddAsync(entity);
        return entity;
    }

    public virtual async Task<bool> UpdateAsync(I id, T entity)
    {
        ArgumentValidatorHelpers.ValidateArgument(entity, nameof(entity));

        try
        {
            _context.Set<T>().Update(entity);
            return true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("An error occurred while updating the entity.", ex);
        }
    }

    public virtual async Task<T[]> AddEntitiesAsync(T[] entities)
    {
        ArgumentValidatorHelpers.ValidateArgument(entities, nameof(entities));

        try
        {
            await _context.Set<T>().AddRangeAsync(entities);
            return entities;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("An error occurred while adding entities.", ex);
        }
    }

    //public virtual async Task<bool> DeleteAsync(I id)
    //{
    //    ArgumentValidatorHelpers.ValidateArgument(id, nameof(id));

    //    var entity = await GetByIdAsync(id);
    //    if (entity == null) throw new KeyNotFoundException("Entity not found.");

    //    _context.Set<T>().Remove(entity);
    //    return true;
    //}


    public virtual async Task<bool> DeleteAsync(I id)
    {
        ArgumentValidatorHelpers.ValidateArgument(id, nameof(id));

        var entity = await GetByIdAsync(id) ?? throw new KeyNotFoundException("Entity not found.");

        if (entity.IsDeleted)
            return true;

        entity.IsDeleted = true;

        return true;
    }


    public virtual bool DeleteAsync(IList<T> entities)
    {
        try
        {

            foreach (var entity in entities)
            {
                ArgumentValidatorHelpers.ValidateArgument(entity.Id, nameof(entity.Id));

                if (entity.IsDeleted)
                    continue;

                entity.IsDeleted = true;

            }

            return true;

        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public virtual async Task<IList<string>> DeleteAsync(Expression<Func<T, bool>> expression)
    {
        ArgumentValidatorHelpers.ValidateArgument(expression, nameof(expression));

        var entities = await GetAllAsync(expression);

        if (entities.Count == 0) throw new ArgumentException("No matching records found.");

        DeleteAsync(entities);

        var ids = entities.Select(e => e.Id?.ToString()).ToList();
        return ids;
    }

    public virtual async Task<IList<T>> GetAllAsync(Expression<Func<T, bool>> expression, params string[] includes)
    {
        var query = _context.Set<T>().Where(expression);

        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        return await query.ToListAsync();
    }

    public virtual async Task<T?> GetAsync(Expression<Func<T, bool>> expression, params string[] includes)
    {
        IQueryable<T> query = _context.Set<T>();

        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        return await query.FirstOrDefaultAsync(expression);
    }

    #endregion

    #region Helper Methods

    private async Task<IList<T>> GetFilteredEntitiesAsync(Dictionary<string, string> fieldValues)
    {
        ArgumentValidatorHelpers.ValidateArgument(fieldValues, nameof(fieldValues));

        var lambda = ExpressionGenerator.GenerateLambda<T>(fieldValues);
        return await _context.Set<T>().Where(lambda).ToListAsync();
    }

    #endregion


}

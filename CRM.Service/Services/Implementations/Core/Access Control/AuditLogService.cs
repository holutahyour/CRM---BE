using System.Text;
using Microsoft.EntityFrameworkCore;

public class AuditLogService : IAuditLogService
{
    private const int ExportRowCap = 50_000;

    private readonly CRM.Data.ApplicationDbContext _db;

    public AuditLogService(CRM.Data.ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result<IList<AuditLogDTO>>> GetAllAsync(AuditLogFilter filter, Guid currentTenantId, string baseUrl)
    {
        var result = new Result<IList<AuditLogDTO>>(false);
        try
        {
            var page = filter.Page < 1 ? 1 : filter.Page;
            var pageSize = filter.PageSize is < 1 or > 500 ? 50 : filter.PageSize;

            var query = BuildQuery(filter, currentTenantId);

            var total = await query.CountAsync();

            var entities = await query
                .OrderByDescending(a => a.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            var items = entities.Select(ToDto).ToList();

            var from = total == 0 ? 0 : (page - 1) * pageSize + 1;
            var to = from == 0 ? 0 : from + items.Count - 1;
            var lastPage = total == 0 ? 1 : (int)Math.Ceiling(total / (double)pageSize);

            result.SetMeta(total, from, to, pageSize, lastPage, baseUrl, baseUrl, "", "", baseUrl);
            result.SetSuccess(items, "Retrieved successfully.");
        }
        catch (Exception ex)
        {
            result.SetError(ex.ToString(), "Error while retrieving audit logs.");
        }

        return result;
    }

    public async Task<Result<byte[]>> ExportCsvAsync(AuditLogFilter filter, Guid currentTenantId)
    {
        var result = new Result<byte[]>(false);
        try
        {
            var entities = await BuildQuery(filter, currentTenantId)
                .OrderByDescending(a => a.Timestamp)
                .Take(ExportRowCap)
                .ToListAsync();
            var items = entities.Select(ToDto).ToList();

            var sb = new StringBuilder();
            sb.AppendLine("Timestamp,ActionType,EntityName,UserId,IpAddress,TenantId,AdditionalInfo");
            foreach (var a in items)
            {
                sb.Append(Csv(a.Timestamp.ToString("o"))).Append(',')
                  .Append(Csv(a.ActionType)).Append(',')
                  .Append(Csv(a.EntityName)).Append(',')
                  .Append(Csv(a.UserId)).Append(',')
                  .Append(Csv(a.IpAddress)).Append(',')
                  .Append(Csv(a.TenantId?.ToString() ?? "")).Append(',')
                  .Append(Csv(a.AdditionalInfo ?? ""))
                  .Append('\n');
            }

            result.SetSuccess(Encoding.UTF8.GetBytes(sb.ToString()), "Export generated.");
        }
        catch (Exception ex)
        {
            result.SetError(ex.ToString(), "Error while exporting audit logs.");
        }

        return result;
    }

    private IQueryable<AuditLog> BuildQuery(AuditLogFilter filter, Guid currentTenantId)
    {
        var query = _db.AuditLogs.IgnoreQueryFilters().Where(a => !a.IsDeleted);

        if (!filter.AllTenants)
            query = query.Where(a => a.TenantId == currentTenantId);

        if (!string.IsNullOrWhiteSpace(filter.EntityName))
            query = query.Where(a => a.EntityName == filter.EntityName);

        if (!string.IsNullOrWhiteSpace(filter.ActionType))
            query = query.Where(a => a.ActionType == filter.ActionType);

        if (!string.IsNullOrWhiteSpace(filter.UserId))
            query = query.Where(a => a.UserId == filter.UserId);

        if (filter.FromDate.HasValue)
            query = query.Where(a => a.Timestamp >= filter.FromDate.Value);

        if (filter.ToDate.HasValue)
            query = query.Where(a => a.Timestamp <= filter.ToDate.Value);

        return query;
    }

    private static AuditLogDTO ToDto(AuditLog a) => new(
        a.Id, a.ActionType, a.EntityName, a.UserId, a.Timestamp,
        a.OldValues, a.NewValues, a.IpAddress, a.AdditionalInfo, a.TenantId);

    private static string Csv(string value)
    {
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        return value;
    }
}

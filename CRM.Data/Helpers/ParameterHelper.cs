namespace CRM.Data.Helpers;

public static class ParameterHelper
{
    /// <summary>
    /// Gets the effective value, converts to T using metadata rules, falls back if invalid/missing.
    /// </summary>
    public static async Task<T> GetEffectiveValueAsync<T>(
        CoreDbContext db,
        string parameterCode,
        string? currentUserCode,
        string? currentHierarchyInstanceCode,
        T fallbackIfMissingOrInvalid)
    {
        // Step 1: Get raw value (priority: user > instance > global)
        string? raw = await GetRawEffectiveValueAsync(db, parameterCode, currentUserCode, currentHierarchyInstanceCode);

        if (string.IsNullOrWhiteSpace(raw))
            return fallbackIfMissingOrInvalid;

        // Step 2: Get metadata rules
        var def = await db.ParameterDefinitions
            .FirstOrDefaultAsync(p => p.ParameterCode == parameterCode);

        if (def == null)
        {
            // Unknown param → best effort
            try { return (T)Convert.ChangeType(raw, typeof(T)); }
            catch { return fallbackIfMissingOrInvalid; }
        }

        // Step 3: Validate + convert based on DataType
        var (isValid, error) = ValidateValue(def, raw);
        if (!isValid)
        {
            // Log warning: error
            return def.DefaultValue != null
                ? (T)Convert.ChangeType(def.DefaultValue, typeof(T))
                : fallbackIfMissingOrInvalid;
        }

        return (T)Convert.ChangeType(raw, typeof(T)); // or use more advanced converter
    }

    private static (bool IsValid, string? Error) ValidateValue(ParameterDefinition def, string value)
    {
        value = value.Trim();

        if (def.IsRequired && string.IsNullOrEmpty(value))
            return (false, "Value is required");

        switch (def.DataType.ToLower())
        {
            case "integer":
                if (!int.TryParse(value, out int i)) return (false, "Invalid integer");
                if (def.MinValue != null && i < int.Parse(def.MinValue)) return (false, $"Below min {def.MinValue}");
                if (def.MaxValue != null && i > int.Parse(def.MaxValue)) return (false, $"Above max {def.MaxValue}");
                break;

            case "boolean":
                var lower = value.ToLower();
                if (!def.AllowedValues?.Split(',').Select(s => s.Trim().ToLower()).Contains(lower) ?? true)
                    return (false, $"Invalid boolean: must be {def.AllowedValues}");
                break;

            case "decimal":
                if (!decimal.TryParse(value, out decimal d)) return (false, "Invalid decimal");
                if (def.MinValue != null && d < decimal.Parse(def.MinValue)) return (false, $"Below min {def.MinValue}");
                if (def.MaxValue != null && d > decimal.Parse(def.MaxValue)) return (false, $"Above max {def.MaxValue}");
                break;

            case "enum":
            case "string":
                if (!string.IsNullOrEmpty(def.AllowedValues) &&
                    !def.AllowedValues.Split(',').Select(s => s.Trim()).Contains(value, StringComparer.OrdinalIgnoreCase))
                    return (false, $"Must be one of: {def.AllowedValues}");
                break;

            // Add 'date', 'guid', etc. as needed
            default:
                return (false, $"Unsupported DataType: {def.DataType}");
        }

        // Regex if defined
        if (!string.IsNullOrEmpty(def.ValidationRegex) && !System.Text.RegularExpressions.Regex.IsMatch(value, def.ValidationRegex))
            return (false, "Does not match pattern");

        return (true, null);
    }

    private static async Task<string?> GetRawEffectiveValueAsync(
        CoreDbContext db,
        string parameterCode,
        string? userCode,
        string? instanceCode)
    {
        // Priority 1: user-specific
        //if (!string.IsNullOrEmpty(userCode))
        //{
        //    var userVal = await db.ParameterValues
        //        .Where(p => p.ParameterCode == parameterCode
        //                 && p.UserCode == userCode
        //                 && p.HierarchyInstanceCode == null)
        //        .Select(p => p.Value)
        //        .FirstOrDefaultAsync();

        //    if (userVal != null) return userVal;
        //}

        //// Priority 2: hierarchy instance
        //if (!string.IsNullOrEmpty(instanceCode))
        //{
        //    var instanceVal = await db.ParameterValues
        //        .Where(p => p.ParameterCode == parameterCode
        //                 && p.UserCode == null
        //                 && p.HierarchyInstanceCode == instanceCode)
        //        .Select(p => p.Value)
        //        .FirstOrDefaultAsync();

        //    if (instanceVal != null) return instanceVal;
        //}

        //// Priority 3: global
        //return await db.ParameterValues
        //    .Where(p => p.ParameterCode == parameterCode
        //             && p.UserCode == null
        //             && p.HierarchyInstanceCode == null)
        //    .Select(p => p.Value)
        //    .FirstOrDefaultAsync();

        return string.Empty;
    }
}

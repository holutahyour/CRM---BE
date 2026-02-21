namespace CRM.Data.Seeds;

public class ParameterDefinitionSeedData
{
    public static ParameterDefinition[] GenerateParameterDefinitionData()
    {
        var data = new List<ParameterDefinition>()
        {
            new()
            {
                ParameterCode = "MAX_OFFICES_PER_USER",
                DisplayName = "Max Offices Per User",
                Description = "Limits the number of concurrent officer roles a single user can hold",
                DataType = "Integer",
                IsRequired = true,
                MinValue = "1",
                MaxValue = "10",
                AllowedValues = null,
                ValidationRegex = null,
                DefaultValue = "2",
                Scope = "Hierarchy",
                ActivityCode = null,
                CreatedOn = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "SYSTEM",
                ModifiedOn = null,
                ModifiedBy = null
            },

            new()
            {
                ParameterCode = "TAX_AUTO_CALC_OFFERING",
                DisplayName = "Auto Calculate Tax for Offering",
                Description = "Determines whether tax is automatically applied to offering collections",
                DataType = "Boolean",
                IsRequired = true,
                MinValue = null,
                MaxValue = null,
                AllowedValues = "Yes,No,True,False",
                ValidationRegex = null,
                DefaultValue = "Yes",
                Scope = "GlobalOnly",
                ActivityCode = "ACT_FIN_TAX",
                CreatedOn = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "SYSTEM",
                ModifiedOn = null,
                ModifiedBy = null
            },

            new()
            {
                ParameterCode = "REM_FREQ",
                DisplayName = "Remittance Frequency",
                Description = "How often chapters should remit funds upward",
                DataType = "Enum",
                IsRequired = true,
                MinValue = null,
                MaxValue = null,
                AllowedValues = "Daily,Weekly,Bi-weekly,Monthly,Quarterly",
                ValidationRegex = null,
                DefaultValue = "Monthly",
                Scope = "Hierarchy",
                ActivityCode = null,
                CreatedOn = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "SYSTEM",
                ModifiedOn = null,
                ModifiedBy = null
            },

            new()
            {
                ParameterCode = "MIN_OFFERING_ALERT",
                DisplayName = "Minimum Offering Alert Threshold",
                Description = "Triggers alert if offering amount falls below this value (NGN)",
                DataType = "Decimal",
                IsRequired = false,
                MinValue = "0.00",
                MaxValue = "999999.99",
                AllowedValues = null,
                ValidationRegex = null,
                DefaultValue = "5000.00",
                Scope = "Hierarchy",
                ActivityCode = null,
                CreatedOn = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "SYSTEM",
                ModifiedOn = null,
                ModifiedBy = null
            }
        };

        return [.. data];
    }
}
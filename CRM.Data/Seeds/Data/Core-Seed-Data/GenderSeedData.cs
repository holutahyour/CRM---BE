using CRM.Base.Common;
using CRM.Domain.Entities;

namespace CRM.Data.Seeds;

public class GenderSeedData
{
    public static Gender[] GenerateGenderData()
    {
        var data = new List<Gender>()
        {
            new() {
                Code = RandomGenerator.RandomString(10),
                Name = "Male",
                Description = "A man or boy",
                CreatedBy = "SYSTEM",
                CreatedOn = DateTime.UtcNow,
                LastModifiedBy = "SYSTEM",
                LastModifiedOn = DateTime.UtcNow
            },
            new() {
                Code =  RandomGenerator.RandomString(10),
                Name = "Female",
                Description = "A woman or girl",
                CreatedBy = "SYSTEM",
                CreatedOn = DateTime.UtcNow,
                LastModifiedBy = "SYSTEM",
                LastModifiedOn = DateTime.UtcNow
            },
        };

        return [.. data];

    }
}

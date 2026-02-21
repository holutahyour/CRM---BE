using Microsoft.Graph.Models;

namespace CRM.Domain.Datas
{
    public class EntraUserData
    {
        public IEnumerable<object> MultipleUsersData(UserCollectionResponse userResponse)
        {
            return userResponse.Value?.Select(user => new
            {
                user.Id,
                user.Photo,
                user.Surname,
                user.DisplayName,
                user.GivenName,
                user.UserPrincipalName,
                user.Mail,
                user.MobilePhone,
                user.Birthday,
                user.StreetAddress,
                user.City,
                user.State,
                user.Country,
                user.JobTitle,
                user.CompanyName,
                user.Department,
                user.EmployeeId,
                user.EmployeeType,
                user.EmployeeExperience,
                user.EmployeeHireDate,
                user.Skills,
                user.BusinessPhones,
                user.AccountEnabled,
            }) ?? Enumerable.Empty<object>();
        }

        public IEnumerable<object> MultipleUsersBasicData(UserCollectionResponse userResponse)
        {
            return userResponse.Value?.Select(user => new
            {
                user.Id,
                user.Surname,
                user.DisplayName,
                user.GivenName,
                user.Mail,
                user.MobilePhone,
                user.StreetAddress,
                user.Country,
                user.JobTitle,
                user.CompanyName,
                user.Department,
                user.EmployeeId,
            }) ?? Enumerable.Empty<object>();
        }

        public object SingleUserData(User user)
        {
            if (user == null) return null;

            return new
            {
                user.Id,
                user.Photo,
                user.Surname,
                user.DisplayName,
                user.GivenName,
                user.UserPrincipalName,
                user.Mail,
                user.MobilePhone,
                user.Birthday,
                user.StreetAddress,
                user.City,
                user.State,
                user.Country,
                user.JobTitle,
                user.CompanyName,
                user.Department,
                user.EmployeeId,
                user.EmployeeType,
                user.EmployeeExperience,
                user.EmployeeHireDate,
                user.Skills,
                user.BusinessPhones,
                user.AccountEnabled,
            };
        }

        public object SingleUserBasicData(User user)
        {
            if (user == null) return null;

            return new
            {
                user.Id,
                user.Surname,
                user.DisplayName,
                user.GivenName,
                user.Mail,
                user.MobilePhone,
                user.StreetAddress,
                user.Country,
                user.JobTitle,
                user.CompanyName,
                user.Department,
                user.EmployeeId,
            };
        }
    }
}

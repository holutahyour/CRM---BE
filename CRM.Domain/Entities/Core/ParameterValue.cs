using CRM.Base.Common.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Domain.Entities
{
    [Table("fr_parameter_values")]
    public class ParameterValue : BaseEntity<long>
    {
        [StringLength(50)]
        public required string ParameterCode { get; set; }

        public required string Value { get; set; }

        [MaxLength(50)]
        public string UserCode { get; set; } = string.Empty;

    }
}

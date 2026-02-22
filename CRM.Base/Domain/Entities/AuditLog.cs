using System.ComponentModel.DataAnnotations;

namespace CRM.Base.Common.Domain.Entities;

[Table("cor_audit_logs")]
public class AuditLog : BaseEntity<long>
{
    public AuditLog() { }

    [MaxLength(50)]
    public string ActionType { get; set; } = string.Empty;   // Create, Update, Delete, Read

    [MaxLength(100)]
    public string EntityName { get; set; } = string.Empty;   // Entity being modified

    [MaxLength(50)]
    public string UserId { get; set; } = string.Empty;       // User performing the action

    public DateTime Timestamp { get; set; }  // When the action occurred

    public string? OldValues { get; set; }    // Previous state of the entity
    public string? NewValues { get; set; }    // New state of the entity
    public string IpAddress { get; set; } = string.Empty;    // IP address of the user
    public string? AdditionalInfo { get; set; }
}

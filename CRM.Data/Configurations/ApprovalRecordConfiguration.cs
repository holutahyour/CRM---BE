using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Data.Configurations;

public class ApprovalRecordConfiguration : IEntityTypeConfiguration<ApprovalRecord>
{
    public void Configure(EntityTypeBuilder<ApprovalRecord> builder)
    {
        builder.ToTable("wf_approval_records");
        builder.Property(x => x.StepName).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Notes).HasMaxLength(1000);
        builder.HasOne(x => x.ActionedByUser).WithMany().HasForeignKey(x => x.ActionedBy).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.WorkflowType, x.EntityId });
    }
}

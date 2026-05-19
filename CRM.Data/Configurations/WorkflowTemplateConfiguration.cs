using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Data.Configurations;

public class WorkflowTemplateConfiguration : IEntityTypeConfiguration<WorkflowTemplate>
{
    public void Configure(EntityTypeBuilder<WorkflowTemplate> builder)
    {
        builder.ToTable("wf_workflow_templates");
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.HasMany(x => x.Steps)
               .WithOne(x => x.Template)
               .HasForeignKey(x => x.WorkflowTemplateId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

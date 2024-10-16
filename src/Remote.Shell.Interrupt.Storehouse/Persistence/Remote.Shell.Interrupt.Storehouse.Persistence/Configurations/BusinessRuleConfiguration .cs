namespace Remote.Shell.Interrupt.Storehouse.Persistence.Configurations;

public class BusinessRuleConfiguration : IEntityTypeConfiguration<BusinessRule>
{
  public void Configure(EntityTypeBuilder<BusinessRule> builder)
  {
    builder.HasOne(br => br.Parent)
           .WithMany(br => br.Children)
           .HasForeignKey(br => br.ParentId)
           .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete on parent

    builder.HasOne(br => br.Assignment)
           .WithMany()
           .HasForeignKey(br => br.AssignmentId);
  }
}
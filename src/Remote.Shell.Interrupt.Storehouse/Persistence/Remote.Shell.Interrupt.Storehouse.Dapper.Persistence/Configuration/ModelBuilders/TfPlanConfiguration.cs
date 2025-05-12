namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Configuration.ModelBuilders;

public class TfPlanConfiguration : IEntityTypeConfiguration<TfPlan>
{
  public void Configure(EntityTypeBuilder<TfPlan> builder)
  {
    builder.ToTable("TfPlans");

    builder.HasKey(x => x.IdTfPlan);

    builder.Property(x => x.Id)
           .HasColumnName("Id")
           .HasColumnType<Guid>("uuid")
           .HasDefaultValueSql("gen_random_uuid()");
    builder.Property(x => x.CreatedAt)
           .HasColumnName("CreatedAt")
           .HasColumnType<DateTime>("timestamptz")
           .HasDefaultValueSql("CURRENT_TIMESTAMP");
    builder.Property(x => x.UpdatedAt)
           .HasColumnName("UpdatedAt")
           .HasColumnType<DateTime?>("timestamptz");
    builder.Property(x => x.IdTfPlan)
           .HasColumnName("IdTfPlan")
           .HasColumnType<int>("integer");
    builder.Property(x => x.NameTfPlan)
           .HasColumnName("NameTfPlan")
           .HasColumnType<string>("text");
    builder.Property(x => x.DescTfPlan)
           .HasColumnName("DescTfPlan")
           .HasColumnType<string?>("text");
  }
}

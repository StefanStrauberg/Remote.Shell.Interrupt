namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Configuration.ModelBuilders;

public class TfPlanConfiguration : IEntityTypeConfiguration<TfPlan>
{
       public void Configure(EntityTypeBuilder<TfPlan> builder)
       {
              builder.ToTable("TfPlans");

              builder.HasKey(x => x.Id);

              builder.HasIndex(x => x.IdTfPlan).IsUnique();

              builder.Property(x => x.Id)
                     .HasColumnName("Id")
                     .HasColumnType("uuid")
                     .HasDefaultValueSql("gen_random_uuid()");
              builder.Property(x => x.CreatedAt)
                     .HasColumnName("CreatedAt")
                     .HasColumnType("timestamptz")
                     .HasDefaultValueSql("CURRENT_TIMESTAMP");
              builder.Property(x => x.UpdatedAt)
                     .HasColumnName("UpdatedAt")
                     .HasColumnType("timestamptz");
              builder.Property(x => x.IdTfPlan)
                     .HasColumnName("IdTfPlan")
                     .HasColumnType("integer")
                     .ValueGeneratedNever();
              builder.Property(x => x.NameTfPlan)
                     .HasColumnName("NameTfPlan")
                     .HasColumnType("text");
              builder.Property(x => x.DescTfPlan)
                     .HasColumnName("DescTfPlan")
                     .HasColumnType("text");
       }
}

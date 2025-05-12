
namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Configuration.ModelBuilders;

public class SPRVlanConfiguration : IEntityTypeConfiguration<SPRVlan>
{
  public void Configure(EntityTypeBuilder<SPRVlan> builder)
  {
    builder.ToTable("SPRVlans");

    builder.HasKey(x => x.Id);

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
    builder.Property(x => x.UseClient)
           .HasColumnName("UseClient")
           .HasColumnType<bool>("boolean");
    builder.Property(x => x.UseCOD)
           .HasColumnName("UseCOD")
           .HasColumnType<bool>("boolean");
    builder.Property(x => x.IdVlan)
           .HasColumnName("IdVlan")
           .HasColumnType<int>("integer");
    builder.Property(x => x.IdClient)
           .HasColumnName("IdClient")
           .HasColumnType<int>("integer");
  }
}

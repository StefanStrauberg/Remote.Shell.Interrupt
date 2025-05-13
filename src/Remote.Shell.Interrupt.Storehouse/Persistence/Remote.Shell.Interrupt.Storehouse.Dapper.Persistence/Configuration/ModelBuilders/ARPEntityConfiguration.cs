namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Configuration.ModelBuilders;

public class ARPEntityConfiguration : IEntityTypeConfiguration<ARPEntity>
{
  public void Configure(EntityTypeBuilder<ARPEntity> builder)
  {
    builder.ToTable("ARPEntities");

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
    builder.Property(x => x.MAC)
           .HasColumnName("MAC")
           .HasColumnType<string>("text");
    builder.Property(x => x.IPAddress)
           .HasColumnName("IPAddress")
           .HasColumnType<string>("text");
    builder.Property(x => x.PortId)
           .HasColumnName("PortId")
           .HasColumnType<Guid>("uuid");
  }
}

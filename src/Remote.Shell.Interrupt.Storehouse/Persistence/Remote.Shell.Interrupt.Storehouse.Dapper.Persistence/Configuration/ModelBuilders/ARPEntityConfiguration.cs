namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Configuration.ModelBuilders;

public class ARPEntityConfiguration : IEntityTypeConfiguration<ARPEntity>
{
  public void Configure(EntityTypeBuilder<ARPEntity> builder)
  {
    builder.ToTable("ARPEntities");

    builder.HasKey(x => x.Id);

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
    builder.Property(x => x.MAC)
           .HasColumnName("MAC")
           .HasColumnType("text");
    builder.Property(x => x.IPAddress)
           .HasColumnName("IPAddress")
           .HasColumnType("text");
    builder.Property(x => x.PortId)
           .HasColumnName("PortId")
           .HasColumnType("uuid");
  }
}

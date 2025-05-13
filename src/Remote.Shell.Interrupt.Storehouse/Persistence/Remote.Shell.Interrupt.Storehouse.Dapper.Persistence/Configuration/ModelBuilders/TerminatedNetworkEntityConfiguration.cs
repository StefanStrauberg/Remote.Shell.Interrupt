namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Configuration.ModelBuilders;

public class TerminatedNetworkEntityConfiguration : IEntityTypeConfiguration<TerminatedNetworkEntity>
{
  public void Configure(EntityTypeBuilder<TerminatedNetworkEntity> builder)
  {
    builder.ToTable("TerminatedNetworkEntities");

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
    builder.Property(x => x.NetworkAddress)
           .HasColumnName("NetworkAddress")
           .HasColumnType("bigint");
    builder.Property(x => x.Netmask)
           .HasColumnName("Netmask")
           .HasColumnType("bigint");
    builder.Property(x => x.PortId)
           .HasColumnName("PortId")
           .HasColumnType("uuid");
  }
}

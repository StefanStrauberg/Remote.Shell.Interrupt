namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Configuration.ModelBuilders;

public class NetworkDeviceConfiguration : IEntityTypeConfiguration<NetworkDevice>
{
  public void Configure(EntityTypeBuilder<NetworkDevice> builder)
  {
    builder.ToTable("NetworkDevices");

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
    builder.Property(x => x.Host)
           .HasColumnName("Host")
           .HasColumnType("text");
    builder.Property(x => x.TypeOfNetworkDevice)
           .HasColumnName("TypeOfNetworkDevice")
           .HasColumnType("integer");
    builder.Property(x => x.GeneralInformation)
           .HasColumnName("GeneralInformation")
           .HasColumnType("text");

    builder.HasMany(x => x.PortsOfNetworkDevice)
           .WithOne()
           .HasForeignKey(x => x.NetworkDeviceId);
  }
}

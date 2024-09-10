namespace Remote.Shell.Interrupt.Storehouse.Persistence.Configurations;

public class NetworkDeviceConfiguration : IEntityTypeConfiguration<NetworkDevice>
{
  public void Configure(EntityTypeBuilder<NetworkDevice> builder)
  {
    builder.HasMany(nd => nd.PortsOfNetworkDevice)
           .WithOne(p => p.NetworkDevice)
           .HasForeignKey(p => p.NetworkDeviceId);
  }
}
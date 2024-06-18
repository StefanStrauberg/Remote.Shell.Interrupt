using Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices;

namespace Remote.Shell.Interrupt.Storehouse.Application.Mappers;

public class RegisterMapper : IRegister
{
  public void Register(TypeAdapterConfig config)
  {
    config.NewConfig<CreateNetworkDeviceCommand, NetworkDevice>()
          .Map(dst => dst.Host, src => src.Host)
          .Map(dst => dst.Vendor, src => src.Vendor)
          .Map(dst => dst.Model, src => src.Model)
          .Map(dst => dst.SoftwareVersion, src => src.SoftwareVersion)
          .Map(dst => dst.GatewayLevel, src => src.GatewayLevel)
          .Map(dst => dst.Interfaces, src => src.Interfaces)
          .Map(dst => dst.VLANs, src => src.VLANs)
          .Map(dst => dst.ARPTable, src => src.ARPTable);
  }
}

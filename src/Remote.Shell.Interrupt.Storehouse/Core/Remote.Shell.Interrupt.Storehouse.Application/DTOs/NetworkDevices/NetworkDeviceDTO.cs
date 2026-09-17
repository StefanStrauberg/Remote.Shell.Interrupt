namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.NetworkDevices;

public class NetworkDeviceDTO : IRegister
{
  public Guid Id { get; set; }
  public string Host { get; set; } = null!;
  public string TypeOfNetworkDevice { get; set; } = string.Empty;
  public string NetworkDeviceName { get; set; } = string.Empty;
  public string GeneralInformation { get; set; } = string.Empty;
  public List<PortDTO> PortsOfNetworkDevice { get; set; } = [];

  void IRegister.Register(TypeAdapterConfig config)
  {
    config.NewConfig<NetworkDevice, NetworkDeviceDTO>()
          .Map(dest => dest.Id,
               src => src.Id)
          .Map(dest => dest.NetworkDeviceName,
               src => src.NetworkDeviceName)
          .Map(dest => dest.Host,
               src => ConvertLongIPAddressToString.Handle(src.Host))
          .Map(dest => dest.TypeOfNetworkDevice,
               src => src.TypeOfNetworkDevice.ToString())
          .Map(dest => dest.GeneralInformation,
               src => src.GeneralInformation)
          .Map(dest => dest.PortsOfNetworkDevice,
               src => src.PortsOfNetworkDevice);
  }
}

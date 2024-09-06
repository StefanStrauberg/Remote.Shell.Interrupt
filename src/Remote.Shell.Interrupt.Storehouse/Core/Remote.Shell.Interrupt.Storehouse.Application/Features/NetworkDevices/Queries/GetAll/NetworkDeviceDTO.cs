namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetAll;

public class NetworkDeviceDTO : IMapWith<NetworkDevice>
{
  public Guid Id { get; set; }
  public required string Host { get; set; }
  public string NetworkDeviceName { get; set; } = string.Empty;
  public string GeneralInformation { get; set; } = string.Empty;
  public List<Port> PortsOfNetworkDevice { get; set; } = [];

  void IMapWith<NetworkDevice>.Mapping(Profile profile)
  {
    profile.CreateMap<NetworkDevice, NetworkDeviceDTO>()
           .ForMember(networkDevice => networkDevice.Id,
                      opt => opt.MapFrom(networkDeviceDTO => networkDeviceDTO.Id))
           .ForMember(networkDevice => networkDevice.NetworkDeviceName,
                      opt => opt.MapFrom(networkDeviceDTO => networkDeviceDTO.NetworkDeviceName))
           .ForMember(networkDevice => networkDevice.Host,
                      opt => opt.MapFrom(networkDeviceDTO => networkDeviceDTO.Host))
           .ForMember(networkDevice => networkDevice.GeneralInformation,
                      opt => opt.MapFrom(networkDeviceDTO => networkDeviceDTO.GeneralInformation))
           .ForMember(networkDevice => networkDevice.PortsOfNetworkDevice,
                      opt => opt.MapFrom(networkDeviceDTO => networkDeviceDTO.PortsOfNetworkDevice));
  }
}

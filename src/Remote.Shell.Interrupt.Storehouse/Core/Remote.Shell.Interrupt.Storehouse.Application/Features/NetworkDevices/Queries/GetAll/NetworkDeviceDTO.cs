namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetAll;

public class NetworkDeviceDTO : IMapWith<NetworkDevice>
{
  public Guid Id { get; set; }
  public required string Host { get; set; }
  public string TypeOfNetworkDevice { get; set; } = string.Empty;
  public string NetworkDeviceName { get; set; } = string.Empty;
  public string GeneralInformation { get; set; } = string.Empty;
  public List<PortDTO> PortsOfNetworkDevice { get; set; } = [];

  void IMapWith<NetworkDevice>.Mapping(Profile profile)
  {
    profile.CreateMap<NetworkDevice, NetworkDeviceDTO>()
           .ForMember(dest => dest.Id,
                      opt => opt.MapFrom(src => src.Id))
           .ForMember(dest => dest.NetworkDeviceName,
                      opt => opt.MapFrom(src => src.NetworkDeviceName))
           .ForMember(dest => dest.Host,
                      opt => opt.MapFrom(src => src.Host))
           .ForMember(dest => dest.TypeOfNetworkDevice,
                      opt => opt.MapFrom(src => src.TypeOfNetworkDevice.ToString()))
           .ForMember(dest => dest.GeneralInformation,
                      opt => opt.MapFrom(src => src.GeneralInformation))
           .ForMember(dest => dest.PortsOfNetworkDevice,
                      opt => opt.MapFrom(src => src.PortsOfNetworkDevice));
  }
}

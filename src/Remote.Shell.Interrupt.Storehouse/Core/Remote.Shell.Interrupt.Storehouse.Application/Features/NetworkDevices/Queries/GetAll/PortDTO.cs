namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetAll;

public class PortDTO : IMapWith<Port>
{
  public int InterfaceNumber { get; set; } // 1
  public string PortName { get; set; } = string.Empty; // "GigabitEthernet0/1"
  public string InterfaceType { get; set; } = string.Empty; // "Ethernet"
  public string InterfaceStatus { get; set; } = string.Empty; // "Up"
  public ulong SpeedOfPort { get; set; } // "1 Gbps"

  public IDictionary<string, HashSet<string>> ARPTableOfPort { get; set; } = null!;
  public IDictionary<string, HashSet<string>> NetworkTableOfPort { get; set; } = null!;

  void IMapWith<Port>.Mapping(Profile profile)
  {
    profile.CreateMap<Port, PortDTO>()
           .ForMember(dest => dest.InterfaceNumber,
                      opt => opt.MapFrom(src => src.InterfaceNumber))
           .ForMember(dest => dest.PortName,
                      opt => opt.MapFrom(src => src.PortName))
           .ForMember(dest => dest.InterfaceType,
                      opt => opt.MapFrom(src => src.InterfaceType.ToDescriptionString()))
           .ForMember(dest => dest.InterfaceStatus,
                      opt => opt.MapFrom(src => src.InterfaceStatus.ToDescriptionString()))
           .ForMember(dest => dest.SpeedOfPort,
                      opt => opt.MapFrom(src => src.SpeedOfPort))
           .ForMember(dest => dest.ARPTableOfPort,
                      opt => opt.MapFrom(src => src.ARPTableOfPort
                                .GroupBy(arp => arp.MAC) // Группируем по MAC-адресу
                                .ToDictionary(
                                    grp => grp.Key, // MAC-адрес как ключ
                                    grp => new HashSet<string>(grp.Select(arp => arp.IPAddress))
                                )))
           .ForMember(dest => dest.NetworkTableOfPort,
                      opt => opt.MapFrom(src => src.NetworkTableOfPort
                                .GroupBy(net => net.IPAddress)
                                .ToDictionary(
                                  grp => grp.Key,
                                  grp => new HashSet<string>(grp.Select(net => net.Netmask))
                                )));
  }
}

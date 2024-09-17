namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetAll;

public class PortDTO : IMapWith<Port>
{
  public int InterfaceNumber { get; set; } // 1
  public string InterfaceName { get; set; } = string.Empty; // "GigabitEthernet0/1"
  public string InterfaceType { get; set; } = string.Empty; // "Ethernet"
  public string InterfaceStatus { get; set; } = string.Empty; // "Up"
  public ulong InterfaceSpeed { get; set; } // "1 Gbps"

  // Aggregation
  public List<Guid> AggregationPorts { get; set; } = [];

  public IDictionary<string, HashSet<string>> ARPTableOfPort { get; set; } = null!;
  public IDictionary<string, HashSet<string>> NetworkTableOfPort { get; set; } = null!;
  public ICollection<VLANDTO> VLANs { get; set; } = null!;

  void IMapWith<Port>.Mapping(Profile profile)
  {
    profile.CreateMap<Port, PortDTO>()
           .ForMember(dest => dest.InterfaceNumber,
                      opt => opt.MapFrom(src => src.InterfaceNumber))
           .ForMember(dest => dest.InterfaceName,
                      opt => opt.MapFrom(src => src.InterfaceName))
           .ForMember(dest => dest.InterfaceType,
                      opt => opt.MapFrom(src => src.InterfaceType.ToDescriptionString()))
           .ForMember(dest => dest.InterfaceStatus,
                      opt => opt.MapFrom(src => src.InterfaceStatus.ToDescriptionString()))
           .ForMember(dest => dest.InterfaceSpeed,
                      opt => opt.MapFrom(src => src.InterfaceSpeed))
           .ForMember(dest => dest.AggregationPorts,
                      opt => opt.MapFrom(src => src.AggregationPorts.Select(x => x.Id)))
           .ForMember(dest => dest.VLANs,
                      opt => opt.MapFrom(src => src.VLANs))
           .ForMember(dest => dest.ARPTableOfPort,
                      opt => opt.MapFrom(src => src.ARPTableOfInterface
                                .GroupBy(arp => arp.MAC) // Группируем по MAC-адресу
                                .ToDictionary(
                                    grp => grp.Key, // MAC-адрес как ключ
                                    grp => new HashSet<string>(grp.Select(arp => arp.IPAddress))
                                )))
           .ForMember(dest => dest.NetworkTableOfPort,
                      opt => opt.MapFrom(src => src.NetworkTableOfInterface
                                .GroupBy(net => net.IPAddress)
                                .ToDictionary(
                                  grp => grp.Key,
                                  grp => new HashSet<string>(grp.Select(net => net.Netmask))
                                )));
  }
}

namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetAll;

public class PortDTO : IMapWith<Port>
{
  public Guid Id { get; set; }
  public int InterfaceNumber { get; set; } // 1
  public string InterfaceName { get; set; } = string.Empty; // "GigabitEthernet0/1"
  public string InterfaceType { get; set; } = string.Empty; // "Ethernet"
  public string InterfaceStatus { get; set; } = string.Empty; // "Up"
  public ulong InterfaceSpeed { get; set; } // "1 Gbps"
  public bool IsAggregated { get; set; }

  public ICollection<PortDTO> AggregatedPorts { get; set; } = [];

  public IDictionary<string, HashSet<string>> ARPTableOfPort { get; set; } = null!;
  public IDictionary<string, HashSet<string>> NetworkTableOfPort { get; set; } = null!;
  public ICollection<VLANDTO> VLANs { get; set; } = null!;

  void IMapWith<Port>.Mapping(Profile profile)
  {
    profile.CreateMap<Port, PortDTO>()
           .ForMember(dest => dest.Id,
                      opt => opt.MapFrom(src => src.Id))
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
           .ForMember(dest => dest.IsAggregated,
                      opt => opt.MapFrom(src => src.AggregatedPorts.Count != 0))
           .ForMember(dest => dest.AggregatedPorts,
                      opt => opt.MapFrom(src => src.AggregatedPorts))
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
                                .GroupBy(net => ConvertToString(net.NetworkAddress))
                                .ToDictionary(
                                  grp => grp.Key,
                                  grp => new HashSet<string>(grp.Select(net => ConvertToString(net.Netmask)))
                                )));
  }

  private static string ConvertToString(uint address)
  {
    var bytes = BitConverter.GetBytes(address);
    Array.Reverse(bytes); // Изменяем порядок байтов
    return new IPAddress(bytes).ToString();
  }
}

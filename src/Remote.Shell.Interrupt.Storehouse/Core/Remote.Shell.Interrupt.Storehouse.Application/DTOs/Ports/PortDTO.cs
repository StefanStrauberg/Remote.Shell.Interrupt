namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Ports;

public class PortDTO : IMapWith<Port>
{
  public Guid Id { get; set; }
  public int InterfaceNumber { get; set; } // 1
  public string InterfaceName { get; set; } = string.Empty; // "GigabitEthernet0/1"
  public string InterfaceType { get; set; } = string.Empty; // "Ethernet"
  public string InterfaceStatus { get; set; } = string.Empty; // "Up"
  public long InterfaceSpeed { get; set; } // "1 Gbps"
  public bool IsAggregated { get; set; }
  public string MACAddress { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;

  public ICollection<PortDTO> AggregatedPorts { get; set; } = [];
  public ICollection<string> MacTable { get; set; } = [];

  public IDictionary<string, HashSet<string>> ARPTableOfPort { get; set; } = null!;
  public IDictionary<string, string> NetworkTableOfPort { get; set; } = null!;
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
           .ForMember(dest => dest.MACAddress,
                      opt => opt.MapFrom(src => src.MACAddress))
           .ForMember(dest => dest.Description,
                      opt => opt.MapFrom(src => src.Description))
           .ForMember(dest => dest.AggregatedPorts,
                      opt => opt.MapFrom(src => src.AggregatedPorts))
           .ForMember(dest => dest.MacTable,
                      opt => opt.MapFrom(src => src.MACTable.Select(x => x.MACAddress)))
           .ForMember(dest => dest.VLANs,
                      opt => opt.MapFrom(src => src.VLANs))
           .ForMember(dest => dest.ARPTableOfPort,
                      opt => opt.MapFrom(src => src.ARPTableOfInterface
                                                   .GroupBy(arp => arp.MAC) // Группируем по MAC-адресу
                                                   .ToDictionary(grp => grp.Key, // MAC-адрес как ключ
                                                                 grp => new HashSet<string>(grp.Select(arp => arp.IPAddress))
                                                   )))
           .ForMember(dest => dest.NetworkTableOfPort,
                      opt => opt.MapFrom(src => src.NetworkTableOfInterface
                                                   .Select(x => new { x.NetworkAddress, x.Netmask })));
  }

  private static string ConvertToString(long address)
  {
    var bytes = BitConverter.GetBytes(address);
    Array.Resize(ref bytes, 4);
    Array.Reverse(bytes); // Изменяем порядок байтов
    return new IPAddress(bytes).ToString();
  }
}

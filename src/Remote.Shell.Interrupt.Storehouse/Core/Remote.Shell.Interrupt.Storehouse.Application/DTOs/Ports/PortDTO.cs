using System.Text.Json.Serialization;

namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Ports;

public class PortDTO : IRegister
{
  public Guid Id { get; set; }
  public int InterfaceNumber { get; set; }
  public string InterfaceName { get; set; } = string.Empty;
  public string InterfaceType { get; set; } = string.Empty;
  public string InterfaceStatus { get; set; } = string.Empty;
  public long InterfaceSpeed { get; set; }
  public bool IsAggregated { get; set; }
  public string MACAddress { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;

  public List<PortDTO> AggregatedPorts { get; set; } = [];
  public List<string> MacTable { get; set; } = [];

  public IDictionary<string, HashSet<string>> ARPTableOfPort { get; set; } = null!;
  public IDictionary<string, string> NetworkTableOfPort { get; set; } = null!;

  // System.Text.Json's default camelCase policy turns "VLANs" into "vlaNs" (it
  // keeps the last letter of an all-caps run uppercase when followed by a
  // lowercase letter, to preserve the word boundary before "s"). Pinned to the
  // "vlans" a consumer would actually expect - see Port.ts on the frontend.
  [JsonPropertyName("vlans")]
  public List<VLANDTO> VLANs { get; set; } = null!;

  public Guid? ParentId { get; set; }

  void IRegister.Register(TypeAdapterConfig config)
  {
    config.NewConfig<Port, PortDTO>()
          .Map(dest => dest.Id,
               src => src.Id)
          .Map(dest => dest.InterfaceNumber,
               src => src.InterfaceNumber)
          .Map(dest => dest.InterfaceName,
               src => src.InterfaceName)
          .Map(dest => dest.InterfaceType,
               src => src.InterfaceType.ToDescriptionString())
          .Map(dest => dest.InterfaceStatus,
               src => src.InterfaceStatus.ToDescriptionString())
          .Map(dest => dest.InterfaceSpeed,
               src => src.InterfaceSpeed)
          .Map(dest => dest.IsAggregated,
               src => src.AggregatedPorts.Count != 0)
          .Map(dest => dest.MACAddress,
               src => src.MACAddress)
          .Map(dest => dest.Description,
               src => src.Description)
          .Map(dest => dest.AggregatedPorts,
               src => src.AggregatedPorts)
          .Map(dest => dest.MacTable,
               src => src.MACTable.Select(x => x.MACAddress))
          .Map(dest => dest.VLANs,
               src => src.VLANs)
          .Map(dest => dest.ARPTableOfPort,
               src => src.ARPTableOfInterface
                          .GroupBy(arp => arp.MAC) // Group by MAC address
                          .ToDictionary(grp => grp.Key, // MAC address as the key
                                        grp => new HashSet<string>(grp.Select(arp => arp.IPAddress))))
          .Map(dest => dest.NetworkTableOfPort,
               src => src.NetworkTableOfInterface.ToDictionary(net => ConvertLongIPAddressToString.Handle(net.NetworkAddress),
                                                                net => ConvertLongIPAddressToString.Handle(net.Netmask)))
          .Map(dest => dest.ParentId,
               src => src.ParentId);
  }
}

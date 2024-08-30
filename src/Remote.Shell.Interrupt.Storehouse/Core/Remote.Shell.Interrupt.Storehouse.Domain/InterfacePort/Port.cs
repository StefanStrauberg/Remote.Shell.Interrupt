namespace Remote.Shell.Interrupt.Storehouse.Domain.InterfacePort;

public class Port : BaseEntity
{
  public int InterfaceNumber { get; set; } // 1
  public string PortName { get; set; } = string.Empty; // "GigabitEthernet0/1"
  public PortType InterfaceType { get; set; } // "Ethernet"
  public PortStatus InterfaceStatus { get; set; } // "Up"
  public ulong SpeedOfPort { get; set; } // "1 Gbps"


  public ICollection<VLAN> VLANsOfPort { get; set; } = [];
  public IDictionary<MACAddress, IPAddress>? ARPTableOfPort { get; set; }
}

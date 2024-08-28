namespace Remote.Shell.Interrupt.Storehouse.Domain.InterfacePort;

public class Port : BaseEntity
{
  public int InterfaceNumber { get; set; } // 1
  public string Name { get; set; } = string.Empty; // "GigabitEthernet0/1"
  public PortType InterfaceType { get; set; } // "Ethernet"
  public PortStatus InterfaceStatus { get; set; } // "Up"
  public ulong Speed { get; set; } // "1 Gbps"


  public ICollection<VLAN> VLANs { get; set; } = [];
  public IDictionary<MACAddress, IPAddress>? ARPTable { get; set; }
}

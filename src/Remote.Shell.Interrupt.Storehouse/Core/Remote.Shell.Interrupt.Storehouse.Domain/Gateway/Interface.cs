using System.Dynamic;

namespace Remote.Shell.Interrupt.Storehouse.Domain.Gateway;

public class Interface : BaseEntity
{
  public int InterfaceNumber { get; set; } // 1
  public string Name { get; set; } = string.Empty; // "GigabitEthernet0/1"
  public InterfaceType InterfaceType { get; set; } // "Ethernet"
  public InterfaceStatus InterfaceStatus { get; set; } // "Up"
  public ulong Speed { get; set; } // "1 Gbps"


  public ICollection<VLAN> VLANs { get; set; } = [];
  public IDictionary<MACAddress, IPAddress>? ARPTable { get; set; }
}

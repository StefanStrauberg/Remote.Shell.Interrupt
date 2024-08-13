using System.Dynamic;

namespace Remote.Shell.Interrupt.Storehouse.Domain.Gateway;

public class Interface : BaseEntity
{
  public int InterfaceNumber { get; set; } // 1
  public string Name { get; set; } = string.Empty; // "GigabitEthernet0/1"
  public InterfaceType InterfaceType { get; set; } // "Ethernet"
  public InterfaceStatus InterfaceStatus { get; set; } // "Up"
  public double Speed { get; set; } // "1 Gbps"

  // Main properties for L2 or L3 gateways
  public ICollection<VLAN> VLANs { get; set; } = [];
  public IDictionary<MACAddress, IPAddress>? ARPTable { get; set; }

  // Navigation property
  public NetworkDevice NetworkDevice { get; set; } = new();
  // Foreign key
  public Guid NetworkDeviceId { get; set; }
}

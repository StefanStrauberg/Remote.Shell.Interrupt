namespace Remote.Shell.Interrupt.Storehouse.Domain.Gateway;

public class Interface
{
  public int InterfaceId { get; set; } // 1
  public string Name { get; set; } = string.Empty; // "GigabitEthernet0/1"
  public string Description { get; set; } = string.Empty; // "Main LAN interface"
  public InterfaceType InterfaceType { get; set; } // "Ethernet"
  public InterfaceStatus InterfaceStatus { get; set; } // "Up"
  public int Speed { get; set; } // "1 Gbps"
  public required MACAddress MacAddress { get; set; } // "00:1A:2B:3C:4D:5E"

  // Navigation property
  public NetworkDevice NetworkDevice { get; set; } = new();
  // Foreign key
  public int NetworkDeviceId { get; set; }
}

namespace Remote.Shell.Interrupt.Storehouse.Domain;

public class ARPEntry
{
  public int Id { get; set; } // 1
  public required IPAddress IPAddress { get; set; } // "192.168.1.100"
  public required MACAddress MACAddress { get; set; } // "AA:BB:CC:DD:EE:FF"
  public DateTime LastSeen { get; set; } // 01.01.1999

  // Foreign key
  public int NetworkDeviceId { get; set; }

  // Navigation property
  public required NetworkDevice NetworkDevice { get; set; }
}
namespace Remote.Shell.Interrupt.Storehouse.Domain.Gateway;

public class ARPEntry : BaseEntity
{
  public required MACAddress MACAddress { get; set; } // "AA:BB:CC:DD:EE:FF"
  public required IPAddress IPAddress { get; set; } // "192.168.1.100"

  // Navigation property
  public required NetworkDevice NetworkDevice { get; set; }
  // Foreign key
  public Guid NetworkDeviceId { get; set; }
}
namespace Remote.Shell.Interrupt.Storehouse.Domain.Gateway;
public class VLAN
{
  public int Id { get; set; } // 10
  public string Name { get; set; } = string.Empty; // "VLAN10"
  public string Description { get; set; } = string.Empty; // "Employees VLAN"
  public string IPAddressRange { get; set; } = string.Empty; // "192.168.10.0/24"

  // Foreign key
  public int NetworkDeviceId { get; set; }

  // Navigation property
  public NetworkDevice NetworkDevice { get; set; } = new();
  public ICollection<Interface> Interfaces { get; set; } = [];
}

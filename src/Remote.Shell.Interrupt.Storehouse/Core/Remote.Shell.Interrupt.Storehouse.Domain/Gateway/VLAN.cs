namespace Remote.Shell.Interrupt.Storehouse.Domain.Gateway;
public class VLAN : BaseEntity
{
  public int VLANNumber { get; set; } // 10
  public string Name { get; set; } = string.Empty; // "VLAN10"

  // Main properties for L2 or L3 gateways
  public ICollection<Interface> Interfaces { get; set; } = [];

  // Navigation property
  public NetworkDevice NetworkDevice { get; set; } = new();
  // Foreign key
  public Guid NetworkDeviceId { get; set; }
}

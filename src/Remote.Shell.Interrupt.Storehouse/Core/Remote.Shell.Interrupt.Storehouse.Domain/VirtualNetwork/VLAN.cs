namespace Remote.Shell.Interrupt.Storehouse.Domain.VirtualNetwork;

/// <summary>
/// Represents a Virtual LAN (VLAN) entity.
/// </summary>
public class VLAN : BaseEntity
{
  /// <summary>
  /// Gets or sets the VLAN tag identifier.
  /// </summary>
  public int VLANTag { get; set; } 

  /// <summary>
  /// Gets or sets the VLAN name.
  /// </summary>
  public string VLANName { get; set; } = string.Empty;

  public IEnumerable<Port> Ports { get; set; } = [];

  public IEnumerable<PortVlan> Enrollments { get; set; } = [];
}

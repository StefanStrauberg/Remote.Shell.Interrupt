namespace Remote.Shell.Interrupt.Storehouse.Domain.VirtualNetwork;

/// <summary>
/// Represents the association between a network port and a VLAN.
/// </summary>
public class PortVlan : BaseEntity
{
  /// <summary>
  /// Gets or sets the unique identifier of the network port.
  /// </summary>
  public Guid PortId { get; set; }

  /// <summary>
  /// Gets or sets the unique identifier of the VLAN.
  /// </summary>
  public Guid VLANId { get; set; }
}

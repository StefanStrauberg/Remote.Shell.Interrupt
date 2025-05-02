namespace Remote.Shell.Interrupt.Storehouse.Domain.InterfacePort;

/// <summary>
/// Represents a MAC address entry associated with a network port.
/// </summary>
public class MACEntity : BaseEntity
{
  /// <summary>
  /// Gets or sets the MAC address recorded for this entity.
  /// </summary>
  public string MACAddress { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the unique identifier of the port to which this MAC address is linked.
  /// </summary>
  public Guid PortId { get; set; }
}
namespace Remote.Shell.Interrupt.Storehouse.Domain.InterfacePort;

/// <summary>
/// Represents an ARP (Address Resolution Protocol) entity associated with a network port.
/// </summary>
public class ARPEntity : BaseEntity
{
  /// <summary>
  /// Gets or sets the MAC address associated with the ARP entry.
  /// </summary>
  public string MAC { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the IP address associated with the ARP entry.
  /// </summary>
  public string IPAddress { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the unique identifier of the port where the ARP entry was recorded.
  /// </summary>
  public Guid PortId { get; set; }
}

namespace Remote.Shell.Interrupt.Storehouse.Domain.Gateway;

/// <summary>
/// Represents a gateway entity with essential network-related properties.
/// </summary>
public class Gate : BaseEntity
{
  /// <summary>
  /// Gets or sets the name of the gateway.
  /// </summary>
  public string Name { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the IP address of the gateway.
  /// </summary>
  public long IPAddress { get; set; }

  /// <summary>
  /// Gets or sets the SNMP community string used for authentication.
  /// </summary>
  public string Community { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the type of network device.
  /// </summary>
  public TypeOfNetworkDevice TypeOfNetworkDevice { get; set; }
}

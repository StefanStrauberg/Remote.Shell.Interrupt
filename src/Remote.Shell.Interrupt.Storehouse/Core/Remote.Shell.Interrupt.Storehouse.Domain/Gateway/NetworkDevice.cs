namespace Remote.Shell.Interrupt.Storehouse.Domain.Gateway;

/// <summary>
/// Represents a network device with essential properties and associated ports.
/// </summary>
public class NetworkDevice : BaseEntity
{
  /// <summary>
  /// Gets or sets the host address of the network device.
  /// </summary>
  public long Host { get; set; }

  /// <summary>
  /// Gets or sets the type of the network device.
  /// </summary>
  public TypeOfNetworkDevice TypeOfNetworkDevice { get; set; }

  /// <summary>
  /// Gets or sets the name of the network device.
  /// </summary>
  public string NetworkDeviceName { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets general information about the network device.
  /// </summary>
  public string GeneralInformation { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the collection of ports associated with the network device.
  /// </summary>
  public List<Port> PortsOfNetworkDevice { get; set; } = [];
}

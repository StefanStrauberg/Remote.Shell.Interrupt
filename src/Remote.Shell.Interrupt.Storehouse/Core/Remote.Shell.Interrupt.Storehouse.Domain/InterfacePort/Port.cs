namespace Remote.Shell.Interrupt.Storehouse.Domain.InterfacePort;

/// <summary>
/// Represents a network interface port with essential properties and related network data.
/// </summary>
public class Port : BaseEntity
{
  /// <summary>
  /// Gets or sets the interface number of the port.
  /// </summary>
  public int InterfaceNumber { get; set; }

  /// <summary>
  /// Gets or sets the interface name.
  /// </summary>
  public string InterfaceName { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the type of the interface.
  /// </summary>
  public PortType InterfaceType { get; set; }

  /// <summary>
  /// Gets or sets the status of the interface.
  /// </summary>
  public PortStatus InterfaceStatus { get; set; } 

  /// <summary>
  /// Gets or sets the speed of the interface.
  /// </summary>
  public long InterfaceSpeed { get; set; } 

  /// <summary>
  /// Gets or sets the MAC address of the port.
  /// </summary>
  public string MACAddress { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the description of the port.
  /// </summary>
  public string Description { get; set; } = string.Empty;

  /// <summary>
  /// Gets or sets the ARP table associated with this interface.
  /// </summary>
  public ICollection<ARPEntity> ARPTableOfInterface { get; set; } = [];

  /// <summary>
  /// Gets or sets the MAC address table of the interface.
  /// </summary>
  public ICollection<MACEntity> MACTable { get; set; } = [];

  /// <summary>
  /// Gets or sets the list of terminated network entities on this interface.
  /// </summary>
  public ICollection<TerminatedNetworkEntity> NetworkTableOfInterface { get; set; } = [];

  /// <summary>
  /// Gets or sets the VLANs associated with this interface.
  /// </summary>
  public ICollection<VLAN> VLANs { get; set; } = [];

  /// <summary>
  /// Gets or sets the aggregated ports associated with this interface.
  /// </summary>
  public ICollection<Port> AggregatedPorts { get; set; } = [];

  /// <summary>
  /// Gets or sets the unique identifier of the network device this port belongs to.
  /// </summary>
  public Guid NetworkDeviceId { get; set; }

  /// <summary>
  /// Gets or sets the unique identifier of the parent port if applicable.
  /// </summary>
  public Guid? ParentPortId { get; set; }
}

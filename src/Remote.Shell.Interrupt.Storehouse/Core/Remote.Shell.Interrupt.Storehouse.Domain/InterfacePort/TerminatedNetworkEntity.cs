namespace Remote.Shell.Interrupt.Storehouse.Domain.InterfacePort;

/// <summary>
/// Represents a terminated network entity associated with a network port.
/// </summary>
public class TerminatedNetworkEntity : BaseEntity
{
  /// <summary>
  /// Gets or sets the network address of the entity.
  /// </summary>
  public long NetworkAddress { get; set; }

  /// <summary>
  /// Gets or sets the subnet mask of the network entity.
  /// </summary>
  public long Netmask { get; set; }

  /// <summary>
  /// Gets or sets the unique identifier of the port where the entity is connected.
  /// </summary>
  public Guid PortId { get; set; }

  /// <summary>
  /// Sets the network address and subnet mask from their string representations.
  /// </summary>
  /// <param name="ipAddress">The IP address as a string.</param>
  /// <param name="netmask">The subnet mask as a string.</param>
  public void SetAddressAndMask(string ipAddress, string netmask)
  {
    NetworkAddress = ConvertToLong(IPAddress.Parse(ipAddress));
    Netmask = ConvertToLong(IPAddress.Parse(netmask));
  }

  /// <summary>
  /// Converts an IP address to its long representation.
  /// </summary>
  /// <param name="ip">The IP address to convert.</param>
  /// <returns>The long integer representation of the IP address.</returns>
  static long ConvertToLong(IPAddress ip)
  {
    return BitConverter.ToUInt32(ip.GetAddressBytes().Reverse().ToArray(), 0);
  }
}

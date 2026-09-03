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
  /// <exception cref="ArgumentException">Thrown when the IP address or netmask format is invalid.</exception>
  public void SetAddressAndMask(string ipAddress, string netmask)
  {
    if (!IPAddress.TryParse(ipAddress, out var ip))
      throw new ArgumentException($"Invalid IP address format: {ipAddress}", nameof(ipAddress));

    if (!IPAddress.TryParse(netmask, out var mask))
      throw new ArgumentException($"Invalid netmask format: {netmask}", nameof(netmask));

    if (ip.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
      throw new ArgumentException("Only IPv4 addresses are supported.", nameof(ipAddress));

    NetworkAddress = ConvertToLong(ip);
    Netmask = ConvertToLong(mask);
  }

  /// <summary>
  /// Converts an IPv4 address to its long representation.
  /// </summary>
  /// <param name="ip">The IPv4 address to convert.</param>
  /// <returns>The long integer representation of the IP address.</returns>
  static long ConvertToLong(IPAddress ip)
    => BitConverter.ToUInt32([.. ip.GetAddressBytes().Reverse()], 0);
}

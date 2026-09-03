namespace Remote.Shell.Interrupt.Storehouse.Application.Helpers;

/// <summary>
/// Converts dotted-decimal IPv4 address strings to their long integer representation.
/// </summary>
public static class ConvertStringIPAddressToLong
{
  /// <summary>
  /// Converts an IPv4 address string to a 32-bit unsigned integer stored as a long.
  /// Each octet is shifted into its corresponding byte position (big-endian network order).
  /// </summary>
  /// <param name="ipAddress">The dotted-decimal IPv4 address (e.g., "192.168.1.1").</param>
  /// <returns>The integer representation of the IP address.</returns>
  /// <exception cref="ArgumentException">Thrown when the input is null or whitespace.</exception>
  /// <exception cref="FormatException">Thrown when the input is not a valid IPv4 address.</exception>
  public static long Handle(string ipAddress)
  {
    if (string.IsNullOrWhiteSpace(ipAddress))
      throw new ArgumentException("IP address cannot be null or empty.", nameof(ipAddress));

    var segments = ipAddress.Split('.');

    if (segments.Length != 4)
      throw new FormatException("Invalid IPv4 format. Expected 4 dot-separated octets.");

    long ipLong = 0;

    for (int i = 0; i < 4; i++)
    {
      if (!byte.TryParse(segments[i], out byte segment))
        throw new FormatException($"Invalid octet: {segments[i]}");

      ipLong |= (long)segment << (8 * (3 - i));
    }

    return ipLong;
  }
}
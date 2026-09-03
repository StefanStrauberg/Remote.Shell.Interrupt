namespace Remote.Shell.Interrupt.Storehouse.Application.Helpers;

/// <summary>
/// Provides methods for formatting MAC addresses from various input formats.
/// </summary>
public static class FormatMACAddress
{
  /// <summary>
  /// Formats a MAC address string by removing spaces and inserting colons between every two characters.
  /// </summary>
  /// <param name="macAddress">The raw MAC address (with or without spaces).</param>
  /// <returns>The formatted MAC address with colon separators (e.g., "AA:BB:CC:DD:EE:FF").</returns>
  /// <remarks>
  /// If the input has an odd number of characters, the last character is silently dropped
  /// because <c>Substring(i * 2, 2)</c> only processes complete pairs.
  /// </remarks>
  public static string Handle(string macAddress)
  {
    var cleanedMac = macAddress.Replace(" ", "");

    var formattedMac = string.Join(":", Enumerable.Range(0, cleanedMac.Length / 2)
                                                  .Select(i => cleanedMac.Substring(i * 2, 2)));

    return formattedMac;
  }

  /// <summary>
  /// Extracts a MAC address from the trailing components of an SNMP OID string.
  /// </summary>
  /// <param name="oid">The OID string ending with 6 byte values (e.g., "1.3.6.1.2.1.17.4.3.1.2.10.20.30.40.50.60").</param>
  /// <returns>The MAC address in colon-separated hex format (e.g., "0A:14:1E:28:32:3C").</returns>
  /// <exception cref="ArgumentException">Thrown when the OID does not start with the expected prefix or has insufficient parts.</exception>
  public static string HandleMACTable(string oid)
  {
    const string prefix = "1.3.6.1.2.1.17.4.3.1.2";

    if (!oid.StartsWith(prefix))
      throw new ArgumentException($"OID must start with {prefix}.", nameof(oid));

    var parts = oid.Split('.');

    // Need at least 8 parts: the prefix (7 segments) + at least 6 for the MAC bytes
    if (parts.Length < 13)
      throw new ArgumentException("OID must contain at least 6 MAC address bytes after the prefix.", nameof(oid));

    byte[] macBytes = new byte[6];
    for (int i = 0; i < 6; i++)
    {
      if (byte.TryParse(parts[parts.Length - 6 + i], out byte value))
        macBytes[i] = value;
      else
        throw new ArgumentException($"Invalid MAC address byte: {parts[parts.Length - 6 + i]}", nameof(oid));
    }

    return string.Join(":", macBytes.Select(b => b.ToString("X2")));
  }
}

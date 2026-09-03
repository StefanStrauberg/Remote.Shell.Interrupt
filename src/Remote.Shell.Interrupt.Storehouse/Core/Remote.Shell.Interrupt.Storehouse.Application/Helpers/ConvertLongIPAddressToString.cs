namespace Remote.Shell.Interrupt.Storehouse.Application.Helpers;

/// <summary>
/// Converts long integer IPv4 addresses back to dotted-decimal string format.
/// </summary>
public static class ConvertLongIPAddressToString
{
  /// <summary>
  /// Converts a long integer (representing a 32-bit IPv4 address) to dotted-decimal notation.
  /// Handles endianness conversion to ensure correct byte ordering.
  /// </summary>
  /// <param name="address">The integer representation of the IPv4 address (0 to 4294967295).</param>
  /// <returns>The dotted-decimal IPv4 address string (e.g., "192.168.1.1").</returns>
  /// <exception cref="ArgumentException">Thrown when the address is outside the valid IPv4 range.</exception>
  public static string Handle(long address)
  {
    if (address < 0 || address > uint.MaxValue)
      throw new ArgumentException($"Invalid IPv4 address value: {address}. Must be between 0 and {uint.MaxValue}.", nameof(address));

    var bytes = BitConverter.GetBytes((uint)address);

    if (BitConverter.IsLittleEndian)
      Array.Reverse(bytes);

    return new IPAddress(bytes).ToString();
  }
}

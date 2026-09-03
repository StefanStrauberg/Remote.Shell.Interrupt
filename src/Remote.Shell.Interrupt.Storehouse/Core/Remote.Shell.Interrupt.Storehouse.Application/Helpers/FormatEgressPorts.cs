namespace Remote.Shell.Interrupt.Storehouse.Application.Helpers;

/// <summary>
/// Provides methods for parsing and formatting egress port data from Juniper and Huawei network devices.
/// </summary>
public static class FormatEgressPorts
{
  static readonly char[] separator = [',', ' '];

  /// <summary>
  /// Parses Juniper egress port data from a comma/space-separated string into an array of port indices.
  /// </summary>
  /// <param name="input">The port data string (e.g., "1, 2, 3, 5").</param>
  /// <returns>An array of port indices, or an empty array if the input is null or whitespace.</returns>
  internal static int[] HandleJuniperData(string input)
  {
    if (string.IsNullOrWhiteSpace(input))
      return [];

    return [.. input.Split(separator, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse)];
  }

  /// <summary>
  /// Parses Huawei hex-encoded egress port data into an array of active port indices.
  /// Each hex byte represents 8 ports, with bits read MSB-first (bit 0 = port N, bit 7 = port N+7).
  /// </summary>
  /// <param name="input">Space-separated hex bytes (e.g., "F0 01 80").</param>
  /// <returns>Zero-based indices of active ports, or an empty array if the input is null or whitespace.</returns>
  public static int[] HandleHuaweiHexString(string input)
  {
    if (string.IsNullOrWhiteSpace(input))
      return [];

    var hexValues = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

    List<int> activePorts = [];

    // Port numbering starts at 1; result uses zero-based indexing (portNumber - 1)
    int portNumber = 1;

    foreach (var hexValue in hexValues)
    {
      byte byteValue = Convert.ToByte(hexValue, 16);
      // Convert.ToString produces MSB-first binary string: 0xF0 → "11110000"
      string binaryString = Convert.ToString(byteValue, 2).PadLeft(8, '0');

      foreach (char bit in binaryString)
      {
        if (bit == '1')
          activePorts.Add(portNumber - 1);

        portNumber++;
      }
    }

    return [.. activePorts];
  }
}

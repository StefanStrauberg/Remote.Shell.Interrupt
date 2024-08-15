namespace Remote.Shell.Interrupt.Storehouse.Domain.Gateway;

public class MACAddress(string macAddress) : BaseEntity
{
  readonly byte[] _mac = ParseMacAddress(macAddress);

  // Method to parse string representation of MAC address into byte array
  private static byte[] ParseMacAddress(string macAddress)
  {
    string[] hexValues = macAddress.Split([':', '-']); // Split by : or -

    if (hexValues.Length != 6)
      throw new ArgumentException("Invalid MAC address format.");

    byte[] bytes = new byte[6];

    for (int i = 0; i < 6; i++)
      bytes[i] = Convert.ToByte(hexValues[i], 16); // Convert each hex pair to byte

    return bytes;
  }

  // Formatting byte array representing of MAC address back into string
  public override string ToString()
    => FormatMacAddress(_mac);

  // Formatting byte array into MAC address string format (e.g., "00:1A:2B:3C:4D:5E")
  private static string FormatMacAddress(byte[] macBytes)
    => string.Join(":", macBytes.Select(b => b.ToString("X2"))); // Format each byte to 2-digit hex with leading zero

  // Getting a byte array representing the MAC address
  public byte[] GetBytes()
    => _mac;
}
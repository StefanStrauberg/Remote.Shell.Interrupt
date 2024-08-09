namespace Remote.Shell.Interrupt.Storehouse.Domain.Gateway;

public class MACAddress(string macAddress)
{
  readonly byte[] macBytes = ParseMacAddress(macAddress);

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

  // Method to format byte array representation of MAC address back into string
  public override string ToString()
    => FormatMacAddress(macBytes);

  // Method to format byte array into MAC address string format (e.g., "00:1A:2B:3C:4D:5E")
  private static string FormatMacAddress(byte[] macBytes)
    => string.Join(":", macBytes.Select(b => b.ToString("X2"))); // Format each byte to 2-digit hex with leading zero

  // Property to get the byte array representation of the MAC address
  public byte[] GetBytes()
    => macBytes;

  // Optional: Method to validate if a given string is a valid MAC address format
  public static bool IsValidMacAddress(string macAddress)
  {
    try
    {
      MACAddress mac = new(macAddress); // Attempt to create MacAddress instance
      return true; // If successful, MAC address format is valid
    }
    catch (ArgumentException)
    {
      return false; // Invalid MAC address format
    }
  }
}
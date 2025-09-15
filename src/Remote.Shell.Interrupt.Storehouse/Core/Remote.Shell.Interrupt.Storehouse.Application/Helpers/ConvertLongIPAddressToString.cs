namespace Remote.Shell.Interrupt.Storehouse.Application.Helpers;

static public class ConvertLongIPAddressToString
{
  public static string Handle(long address)
  {
    if (address < 0 || address > uint.MaxValue)
      throw new ArgumentException($"Invalid IPv4 address value: {address}", nameof(address));

    var bytes = BitConverter.GetBytes((uint)address); // cast to uint to ensure 4 bytes

    if (BitConverter.IsLittleEndian)
      Array.Reverse(bytes);

    return new IPAddress(bytes).ToString();
  }
}

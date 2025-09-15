namespace Remote.Shell.Interrupt.Storehouse.Application.Helpers;

static public class ConvertStringIPAddressToLong
{
  public static long Handle(string ipAddress)
  {
    if (string.IsNullOrWhiteSpace(ipAddress))
      throw new ArgumentException("IP address cannot be null or empty.");

    var segments = ipAddress.Split('.');

    if (segments.Length != 4)
      throw new FormatException("Invalid IPv4 format.");

    long ipLong = 0;

    for (int i = 0; i < 4; i++)
    {
      if (!byte.TryParse(segments[i], out byte segment))
        throw new FormatException($"Invalid segment: {segments[i]}");

      ipLong |= (long)segment << (8 * (3 - i));
    }

    return ipLong;
  }
}
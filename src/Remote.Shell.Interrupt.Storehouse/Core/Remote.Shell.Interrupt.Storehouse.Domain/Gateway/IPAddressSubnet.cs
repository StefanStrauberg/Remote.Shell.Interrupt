namespace Remote.Shell.Interrupt.Storehouse.Domain.Gateway;

public class IPAddressSubnet : BaseEntity
{
  readonly IPAddress _networkAddress;
  readonly IPAddress _broadcastAddress;
  readonly IPAddress _subnetMask;
  readonly IPAddress _wildcardMask;
  readonly int _prefixLength;

  public IPAddress NetworkAddress => _networkAddress;
  public IPAddress BroadcastAddress => _broadcastAddress;
  public IPAddress SubnetMask => _subnetMask;
  public IPAddress WildcardMask => _wildcardMask;
  public int TotalHosts => CalculateTotalHosts();
  public int UsableHosts => TotalHosts - 2; // Subtracting network and broadcast addresses
  public string BinarySubnetMask => GetBinaryString(_subnetMask);
  public string CIDRNotation => $"/{_prefixLength}";
  public string IpClass => GetIpClass(_ipAddress);
  public string IpType => IsPrivateIp(_ipAddress) ? "Private" : "Public";


  public IPAddressSubnet(string cidr)
  {
    var parts = cidr.Split('/');

    if (parts.Length != 2)
      throw new ArgumentException("Invalid CIDR format.");

    _ipAddress = IPAddress.Parse(parts[0]);
    _prefixLength = int.Parse(parts[1]);

    if (_prefixLength < 0 || _prefixLength > 32)
      throw new ArgumentException("Prefix length must be between 0 and 32.");

    _subnetMask = GetSubnetMask(_prefixLength);
    _networkAddress = GetNetworkAddress(_ipAddress, _subnetMask);
    _broadcastAddress = GetBroadcastAddress(_networkAddress, _subnetMask);
    _wildcardMask = GetWildcardMask(_subnetMask);
  }

  private static IPAddress GetSubnetMask(int prefixLength)
  {
    uint mask = 0xffffffff << (32 - prefixLength);
    return new IPAddress(BitConverter.GetBytes(mask).Reverse().ToArray());
  }

  private static IPAddress GetNetworkAddress(IPAddress ipAddress, IPAddress subnetMask)
  {
    byte[] ipBytes = ipAddress.GetAddressBytes();
    byte[] maskBytes = subnetMask.GetAddressBytes();
    byte[] networkBytes = new byte[ipBytes.Length];

    for (int i = 0; i < ipBytes.Length; i++)
      networkBytes[i] = (byte)(ipBytes[i] & maskBytes[i]);

    return new IPAddress(networkBytes);
  }

  private static IPAddress GetBroadcastAddress(IPAddress networkAddress, IPAddress subnetMask)
  {
    byte[] networkBytes = networkAddress.GetAddressBytes();
    byte[] maskBytes = subnetMask.GetAddressBytes();
    byte[] broadcastBytes = new byte[networkBytes.Length];

    for (int i = 0; i < networkBytes.Length; i++)
      broadcastBytes[i] = (byte)(networkBytes[i] | ~maskBytes[i]);

    return new IPAddress(broadcastBytes);
  }

  private static IPAddress GetWildcardMask(IPAddress subnetMask)
  {
    byte[] maskBytes = subnetMask.GetAddressBytes();
    byte[] wildcardBytes = new byte[maskBytes.Length];

    for (int i = 0; i < maskBytes.Length; i++)
      wildcardBytes[i] = (byte)(~maskBytes[i]);

    return new IPAddress(wildcardBytes);
  }

  private int CalculateTotalHosts()
    => (int)Math.Pow(2, 32 - _prefixLength);

  private static string GetBinaryString(IPAddress ipAddress)
  {
    var bytes = ipAddress.GetAddressBytes();
    var sb = new StringBuilder();

    foreach (var b in bytes)
      sb.Append(Convert.ToString(b, 2).PadLeft(8, '0')).Append('.');

    return sb.ToString().TrimEnd('.');
  }

  private static string GetIpClass(IPAddress ipAddress)
  {
    var firstOctet = ipAddress.GetAddressBytes()[0];
    if (firstOctet >= 1 && firstOctet <= 126)
      return "A";
    if (firstOctet >= 128 && firstOctet <= 191)
      return "B";
    if (firstOctet >= 192 && firstOctet <= 223)
      return "C";
    if (firstOctet >= 224 && firstOctet <= 239)
      return "D";
    if (firstOctet >= 240 && firstOctet <= 255)
      return "E";

    return "Unknown";
  }

  private static bool IsPrivateIp(IPAddress ipAddress)
  {
    var bytes = ipAddress.GetAddressBytes();

    return (bytes[0] == 10) ||
           (bytes[0] == 172 && (bytes[1] >= 16 && bytes[1] <= 31)) ||
           (bytes[0] == 192 && bytes[1] == 168);
  }

  public string GetUsableHostRange()
  {
    if (UsableHosts <= 0)
      return "No usable hosts";

    var startHost = new IPAddress(GetUsableStartHostAddress());
    var endHost = new IPAddress(GetUsableEndHostAddress());
    return $"{startHost} - {endHost}";
  }

  private byte[] GetUsableStartHostAddress()
  {
    byte[] networkBytes = _networkAddress.GetAddressBytes();
    networkBytes[^1] += 1;
    return networkBytes;
  }

  private byte[] GetUsableEndHostAddress()
  {
    byte[] broadcastBytes = _broadcastAddress.GetAddressBytes();
    broadcastBytes[^1] -= 1;
    return broadcastBytes;
  }
}
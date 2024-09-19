namespace Remote.Shell.Interrupt.Storehouse.Domain.InterfacePort;

public class TerminatedNetworkEntity : BaseEntity
{
  public uint NetworkAddress { get; set; }
  public uint Netmask { get; set; }

  public Guid PortId { get; set; }
  public Port Port { get; set; } = null!;

  // Метод для установки сетевого адреса и маски из строкового представления
  public void SetAddressAndMask(string ipAddress, string netmask)
  {
    NetworkAddress = ConvertToUInt32(IPAddress.Parse(ipAddress));
    Netmask = ConvertToUInt32(IPAddress.Parse(netmask));
  }

  private static uint ConvertToUInt32(IPAddress ip)
  {
    return BitConverter.ToUInt32(ip.GetAddressBytes().Reverse().ToArray(), 0);
  }
}

namespace Remote.Shell.Interrupt.Storehouse.Domain.InterfacePort;

public class TerminatedNetworkEntity : BaseEntity
{
  public long NetworkAddress { get; set; }
  public long Netmask { get; set; }

  public Guid PortId { get; set; }

  // Метод для установки сетевого адреса и маски из строкового представления
  public void SetAddressAndMask(string ipAddress, string netmask)
  {
    NetworkAddress = ConvertToLong(IPAddress.Parse(ipAddress));
    Netmask = ConvertToLong(IPAddress.Parse(netmask));
  }

  private static long ConvertToLong(IPAddress ip)
  {
    return BitConverter.ToUInt32(ip.GetAddressBytes().Reverse().ToArray(), 0);
  }
}

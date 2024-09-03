namespace Remote.Shell.Interrupt.Storehouse.Domain.Gateway;

public class NetworkDevice : BaseEntity
{
  public IPAddress? Host { get; set; } // "192.168.101.29" 
  public string NetworkDeviceName { get; set; } = string.Empty; // "Huawei"
  public string GeneralInformation { get; set; } = string.Empty;
  public List<Port> PortsOfNetworkDevice { get; set; } = [];
}

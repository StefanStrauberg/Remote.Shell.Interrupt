namespace Remote.Shell.Interrupt.Storehouse.Domain;

public class NetworkDevice : BaseEntity
{
  public IPAddress? Host { get; set; } // "192.168.101.29"
  public string Vendor { get; set; } = string.Empty; // "Huawei"
  public string Model { get; set; } = string.Empty; // CE6863E-48S6CQ
  public string SoftwareVersion { get; set; } = string.Empty; // "8.220"
  public GatewayLevel GatewayLevel { get; set; } // L2

  // Navigation properties
  public ICollection<Interface> Interfaces { get; set; } = [];
  public ICollection<VLAN> VLANs { get; set; } = [];
  public ICollection<ARPEntry> ARPTable { get; set; } = [];
}

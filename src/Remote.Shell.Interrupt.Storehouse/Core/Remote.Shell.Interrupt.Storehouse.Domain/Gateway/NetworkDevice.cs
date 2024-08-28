namespace Remote.Shell.Interrupt.Storehouse.Domain.Gateway;

public class NetworkDevice : BaseEntity
{
  // Set manually
  public IPAddress? Host { get; set; } // "192.168.101.29" 
  // Set manually
  public string Name { get; set; } = string.Empty; // "Huawei"
  // Navigation properties
  public ICollection<Port> Ports { get; set; } = [];
}

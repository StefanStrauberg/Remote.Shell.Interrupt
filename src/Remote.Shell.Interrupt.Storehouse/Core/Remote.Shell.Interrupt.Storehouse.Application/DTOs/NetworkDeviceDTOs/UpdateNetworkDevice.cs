namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.NetworkDeviceDTOs;

public class UpdateNetworkDevice
{
  public Guid Id { get; set; }
  public IPAddress? Host { get; set; }
  public string Name { get; set; } = string.Empty;
  // Navigation properties
  public ICollection<Port> Ports { get; set; } = [];
}

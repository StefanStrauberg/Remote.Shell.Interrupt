namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.NetworkDeviceDTOs;

public class NetworkDeviceDTO
{
  public Guid Id { get; set; }
  public IPAddress? Host { get; set; }
  public string Name { get; set; } = string.Empty;
  public string GeneralInformation { get; set; } = string.Empty;
  public ICollection<Port> Ports { get; set; } = [];
}

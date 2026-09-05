namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.NetworkDevices;

public class CompoundObjectDTO
{
  public IEnumerable<NetworkDeviceDTO> NetworkDevices { get; set; } = [];
  public IEnumerable<DetailClientDTO> Clients { get; set; } = [];
}

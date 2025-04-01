namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetByExpression;

public class CompoundObjectDTO
{
  public IEnumerable<NetworkDeviceDTO> NetworkDevices { get; set; } = [];
  public IEnumerable<DetailClientDTO> Clients { get; set; } = [];
}

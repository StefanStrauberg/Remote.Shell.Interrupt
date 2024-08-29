namespace Remote.Shell.Interrupt.Storehouse.API.Controllers;

public class NetworkDevicesController(ISender sender) : BaseAPIController
{
  readonly ISender _sender = sender
    ?? throw new ArgumentNullException(nameof(sender));

  [HttpGet]
  [ProducesResponseType(typeof(IEnumerable<NetworkDeviceDTO>), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetNetworkDevices(CancellationToken cancellationToken)
    => Ok(await _sender.Send(new GetNetworkDevicesQuery(),
                             cancellationToken));

  [HttpGet("{id}")]
  [ProducesResponseType(typeof(NetworkDeviceDTO), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetNetworkDeviceById(Guid id,
                                                        CancellationToken cancellationToken)
  => Ok(await _sender.Send(new GetNetworkDeviceByExpressionQuery(x => x.Id == id),
                           cancellationToken));
}

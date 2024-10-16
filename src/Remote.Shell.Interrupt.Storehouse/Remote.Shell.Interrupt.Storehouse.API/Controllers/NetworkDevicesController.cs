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
  [ProducesResponseType(typeof(List<NetworkDeviceDTO>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetNetworkDevicesByID(Guid id,
                                                         CancellationToken cancellationToken)
    => Ok(await _sender.Send(new GetNetworkDeviceByIdQuery(id),
                             cancellationToken));

  [HttpGet("{address}")]
  [ProducesResponseType(typeof(List<NetworkDeviceDTO>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetNetworkDevicesByIP(string address,
                                                         CancellationToken cancellationToken)
    => Ok(await _sender.Send(new GetNetworkDevicesByIPQuery(address),
                             cancellationToken));

  [HttpPost]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public async Task<IActionResult> CreateNetworkDevice(CreateNetworkDeviceCommand createNetworkDeviceCommand,
                                                      CancellationToken cancellationToken)
    => Ok(await _sender.Send(createNetworkDeviceCommand,
                             cancellationToken));

  [HttpDelete("{id}")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> DeleteNetworkDeviceById(Guid id,
                                                           CancellationToken cancellationToken)
    => Ok(await _sender.Send(new DeleteNetworkDeviceByExpressionCommand(x => x.Id == id),
                             cancellationToken));
}

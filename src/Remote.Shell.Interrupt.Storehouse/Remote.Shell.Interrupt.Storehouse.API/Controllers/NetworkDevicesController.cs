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
  [ProducesResponseType(typeof(IEnumerable<NetworkDeviceDTO>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetNetworkDevicesById(Guid id,
                                                         CancellationToken cancellationToken)
    => Ok(await _sender.Send(new GetNetworkDeviceByIdQuery(id),
                             cancellationToken));

  [HttpGet("{address}")]
  [ProducesResponseType(typeof(IEnumerable<NetworkDeviceDTO>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetNetworkDevicesByIP(string address,
                                                         CancellationToken cancellationToken)
    => Ok(await _sender.Send(new GetNetworkDevicesByIPQuery(address),
                             cancellationToken));

  [HttpGet("{VLANTag}")]
  [ProducesResponseType(typeof(IEnumerable<NetworkDeviceDTO>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetNetworkDevicesByVlanTag(int VLANTag,
                                                              CancellationToken cancellationToken)
    => Ok(await _sender.Send(new GetNetworkDeviceByVlanTagQuery(VLANTag),
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
    => Ok(await _sender.Send(new DeleteNetworkDeviceByIdCommand(id),
                             cancellationToken));
}

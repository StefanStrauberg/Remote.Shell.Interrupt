namespace Remote.Shell.Interrupt.Storehouse.API.Controllers;

public class NetworkDevicesController : BaseAPIController
{
  [HttpGet]
  [ProducesResponseType(typeof(IEnumerable<NetworkDeviceDTO>), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetNetworkDevices(CancellationToken cancellationToken)
    => Ok(await Sender.Send(new GetNetworkDevicesQuery(),
                            cancellationToken));

  [HttpGet("{id}")]
  [ProducesResponseType(typeof(IEnumerable<NetworkDeviceDTO>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetNetworkDevicesById(Guid id,
                                                         CancellationToken cancellationToken)
    => Ok(await Sender.Send(new GetNetworkDeviceByIdQuery(id),
                            cancellationToken));

  [HttpGet("{address}")]
  [ProducesResponseType(typeof(CompoundObjectDTO), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetNetworkDevicesByIP(string address,
                                                         CancellationToken cancellationToken)
    => Ok(await Sender.Send(new GetNetworkDevicesByIPQuery(address),
                            cancellationToken));

  [HttpGet("{VLANTag}")]
  [ProducesResponseType(typeof(CompoundObjectDTO), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetNetworkDevicesByVlanTag(int VLANTag,
                                                              CancellationToken cancellationToken)
    => Ok(await Sender.Send(new GetNetworkDeviceByVlanTagQuery(VLANTag),
                            cancellationToken));

  [HttpGet("{OrganizationName}")]
  [ProducesResponseType(typeof(CompoundObjectDTO), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetNetworkDevicesByOrganizationName(string OrganizationName,
                                                                       CancellationToken cancellationToken)
    => Ok(await Sender.Send(new GetNetworkDeviceByOrganizationNameQuery(OrganizationName),
                            cancellationToken));

  [HttpPost]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public async Task<IActionResult> CreateNetworkDevice(CreateNetworkDeviceCommand createNetworkDeviceCommand,
                                                       CancellationToken cancellationToken)
    => Ok(await Sender.Send(createNetworkDeviceCommand,
                            cancellationToken));

  [HttpDelete("{id}")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> DeleteNetworkDeviceById(Guid id,
                                                           CancellationToken cancellationToken)
    => Ok(await Sender.Send(new DeleteNetworkDeviceByIdCommand(id),
                            cancellationToken));
}

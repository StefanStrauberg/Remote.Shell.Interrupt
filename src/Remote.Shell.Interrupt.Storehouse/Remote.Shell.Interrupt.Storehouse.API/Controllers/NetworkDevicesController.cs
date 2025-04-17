namespace Remote.Shell.Interrupt.Storehouse.API.Controllers;

public class NetworkDevicesController : BaseAPIController
{
  [HttpGet]
  [ProducesResponseType(typeof(IEnumerable<NetworkDeviceDTO>), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetNetworkDevices([FromQuery] RequestParameters requestParameters,
                                                     CancellationToken cancellationToken)
  {
    var result = await Sender.Send(new GetNetworkDevicesQuery(requestParameters),
                                   cancellationToken);
    var metadata = new
    {
        result.TotalCount,
        result.PageSize,
        result.CurrentPage,
        result.TotalPages,
        result.HasNext,
        result.HasPrevious
    };
    Response.Headers
            .Append("X-Pagination",
                    JsonSerializer.Serialize(metadata));
    return Ok(result);
  }

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

  [HttpDelete]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> DeleteNetworkDevices(Guid id,
                                                        CancellationToken cancellationToken)
    => Ok(await Sender.Send(new DeleteNetworkDevicesCommand(),
                            cancellationToken));
}

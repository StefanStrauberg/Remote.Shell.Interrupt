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

  [HttpGet("{address}")]
  [ProducesResponseType(typeof(NetworkDeviceDTO), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetNetworkDeviceByIP(string address,
                                                        CancellationToken cancellationToken)
  {
    var ipToCheck = IPAddress.Parse(address);
    var ipToCheckNum = BitConverter.ToUInt32(ipToCheck.GetAddressBytes().Reverse().ToArray(), 0);
    var request = new GetNetworkDeviceByExpressionQuery(
        x => x.PortsOfNetworkDevice.Any(port =>
            port.NetworkTableOfInterface.Any(network => (ipToCheckNum & network.Netmask) ==
                                                        (network.NetworkAddress & network.Netmask))
        )
    );
    var result = await _sender.Send(request, cancellationToken);
    return Ok(result);
  }

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

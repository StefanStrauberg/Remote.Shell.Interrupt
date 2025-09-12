namespace Remote.Shell.Interrupt.Storehouse.API.Controllers;

/// <summary>
/// Provides API endpoints for managing network device records, including queries, creation, and deletion.
/// </summary>
public class NetworkDevicesController(ISender sender) : BaseAPIController(sender)
{
  /// <summary>
  /// Retrieves a filtered and paginated list of network device records.
  /// </summary>
  /// <param name="requestParameters">Filtering and pagination options sent via query string.</param>
  /// <param name="cancellationToken">Token to cancel the request if needed.</param>
  /// <returns>
  /// A paginated collection of <see cref="NetworkDeviceDTO"/> records.
  /// Pagination metadata is returned in the <c>X-Pagination</c> response header.
  /// </returns>
  [HttpGet]
  [ProducesResponseType(typeof(IEnumerable<NetworkDeviceDTO>), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetNetworkDevicesByFilter([FromQuery] RequestParameters requestParameters,
                                                             CancellationToken cancellationToken)
  {
    var result = await Sender.Send(new GetNetworkDevicesByFilterQuery(requestParameters), cancellationToken);
    var metadata = new PaginationMetadata()
    {
      TotalCount = result.TotalCount,
      PageSize = result.PageSize,
      CurrentPage = result.CurrentPage,
      TotalPages = result.TotalPages,
      HasNext = result.HasNext,
      HasPrevious = result.HasPrevious
    };
    Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));
    return Ok(result);
  }

  /// <summary>
  /// Retrieves a single network device by its unique identifier.
  /// </summary>
  /// <param name="id">The unique ID of the network device.</param>
  /// <param name="cancellationToken">Token to cancel the request if needed.</param>
  /// <returns>
  /// A <see cref="NetworkDeviceDTO"/> if found, or an <see cref="ApiErrorResponse"/> with <c>404 Not Found</c> if missing.
  /// </returns>
  [HttpGet("{id}")]
  [ProducesResponseType(typeof(IEnumerable<NetworkDeviceDTO>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetNetworkDeviceById(Guid id, CancellationToken cancellationToken)
    => Ok(await Sender.Send(new GetNetworkDeviceByIdQuery(id), cancellationToken));

  /// <summary>
  /// Retrieves compound network device information associated with the specified VLAN tag.
  /// </summary>
  /// <param name="VLANTag">The VLAN tag used to identify associated network devices.</param>
  /// <param name="cancellationToken">Token to cancel the request if needed.</param>
  /// <returns>
  /// A <see cref="CompoundObjectDTO"/> with aggregated data if found, or <see cref="ApiErrorResponse"/> if missing.
  /// </returns>
  [HttpGet("{VLANTag}")]
  [ProducesResponseType(typeof(CompoundObjectDTO), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetNetworkDevicesByVlanTag(int VLANTag, CancellationToken cancellationToken)
    => Ok(await Sender.Send(new GetCompundDataByVlanTagQuery(VLANTag), cancellationToken));

  /// <summary>
  /// Creates a new network device record.
  /// </summary>
  /// <param name="createNetworkDeviceCommand">Command containing device creation data.</param>
  /// <param name="cancellationToken">Token to cancel the request if needed.</param>
  /// <returns><see cref="StatusCodes.Status200OK"/> on success.</returns>
  [HttpPost]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public async Task<IActionResult> CreateNetworkDevice(CreateNetworkDeviceCommand createNetworkDeviceCommand,
                                                       CancellationToken cancellationToken)
    => Ok(await Sender.Send(createNetworkDeviceCommand, cancellationToken));

  /// <summary>
  /// Deletes a single network device by its unique identifier.
  /// </summary>
  /// <param name="id">The ID of the device to delete.</param>
  /// <param name="cancellationToken">Token to cancel the request if needed.</param>
  /// <returns>
  /// <see cref="StatusCodes.Status200OK"/> on success, or <see cref="ApiErrorResponse"/> with <c>404 Not Found</c> if missing.
  /// </returns>
  [HttpDelete("{id}")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> DeleteNetworkDeviceById(Guid id, CancellationToken cancellationToken)
    => Ok(await Sender.Send(new DeleteNetworkDeviceByIdCommand(id), cancellationToken));

  /// <summary>
  /// Deletes all existing network device records from the system.
  /// </summary>
  /// <param name="cancellationToken">Token to cancel the request if needed.</param>
  /// <returns><see cref="StatusCodes.Status200OK"/> on success, or <see cref="ApiErrorResponse"/> if deletion fails.</returns>
  [HttpDelete]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> DeleteNetworkDevices(CancellationToken cancellationToken)
    => Ok(await Sender.Send(new DeleteAllNetworkDevicesCommand(), cancellationToken));
}

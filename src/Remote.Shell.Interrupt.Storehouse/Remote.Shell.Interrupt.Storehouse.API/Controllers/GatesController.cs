namespace Remote.Shell.Interrupt.Storehouse.API.Controllers;

/// <summary>
/// Provides API endpoints for retrieving, creating, updating, and deleting gate entities.
/// </summary>
public class GatesController : BaseAPIController
{
  /// <summary>
  /// Retrieves a filtered and paginated list of gate entities.
  /// </summary>
  /// <param name="requestParameters">Contains filtering and pagination options.</param>
  /// <param name="cancellationToken">Token to cancel the request if needed.</param>
  /// <returns>
  /// A paginated collection of <see cref="GateDTO"/>s with metadata in the <c>X-Pagination</c> response header.
  /// </returns>
  [HttpGet]
  [ProducesResponseType(typeof(IEnumerable<GateDTO>), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetGatesByFilter([FromQuery] RequestParameters requestParameters,
                                                    CancellationToken cancellationToken)
  {
    var result = await Sender.Send(new GetGatesByFilterQuery(requestParameters), cancellationToken);
    var metadata = new
    {
      result.TotalCount,
      result.PageSize,
      result.CurrentPage,
      result.TotalPages,
      result.HasNext,
      result.HasPrevious
    };
    Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));
    return Ok(result);
  }

  /// <summary>
  /// Retrieves a single gate entity by its unique identifier.
  /// </summary>
  /// <param name="id">The unique ID of the gate to retrieve.</param>
  /// <param name="cancellationToken">Token to cancel the request if needed.</param>
  /// <returns>
  /// A <see cref="GateDTO"/> object if found, or <see cref="ApiErrorResponse"/> with status <c>404</c> if not.
  /// </returns>
  [HttpGet("{id}")]
  [ProducesResponseType(typeof(GateDTO), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetGateById(Guid id, CancellationToken cancellationToken)
    => Ok(await Sender.Send(new GetGateByIdQuery(id), cancellationToken));

  /// <summary>
  /// Creates a new gate entity using the provided data.
  /// </summary>
  /// <param name="createGateDTO">The data required to create the gate.</param>
  /// <param name="cancellationToken">Token to cancel the request if needed.</param>
  /// <returns><see cref="StatusCodes.Status200OK"/> upon successful creation.</returns>
  [HttpPost]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public async Task<IActionResult> CreateGate([FromBody] CreateGateDTO createGateDTO,
                                              CancellationToken cancellationToken)
    => Ok(await Sender.Send(new CreateGateCommand(createGateDTO), cancellationToken));

  /// <summary>
  /// Deletes a gate entity by its unique identifier.
  /// </summary>
  /// <param name="Id">The ID of the gate to delete.</param>
  /// <param name="cancellationToken">Token to cancel the request if needed.</param>
  /// <returns>
  /// <see cref="StatusCodes.Status200OK"/> on successful deletion, or <see cref="ApiErrorResponse"/> with status <c>404</c> if not found.
  /// </returns>
  [HttpDelete("{Id}")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> DeleteGateById(Guid Id, CancellationToken cancellationToken)
    => Ok(await Sender.Send(new DeleteGateCommand(Id), cancellationToken));

  /// <summary>
  /// Updates an existing gate entity using the provided data.
  /// </summary>
  /// <param name="updateGateDTO">The data used to update the gate.</param>
  /// <param name="cancellationToken">Token to cancel the request if needed.</param>
  /// <returns>
  /// <see cref="StatusCodes.Status200OK"/> on success, or <see cref="ApiErrorResponse"/> with status <c>404</c> if the gate was not found.
  /// </returns>
  [HttpPut]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> UpdateGate([FromBody] UpdateGateDTO updateGateDTO,
                                              CancellationToken cancellationToken)
    => Ok(await Sender.Send(new UpdateGateCommand(updateGateDTO), cancellationToken));
}

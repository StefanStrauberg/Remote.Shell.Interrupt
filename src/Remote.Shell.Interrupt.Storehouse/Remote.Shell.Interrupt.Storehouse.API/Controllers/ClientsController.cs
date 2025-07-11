namespace Remote.Shell.Interrupt.Storehouse.API.Controllers;

/// <summary>
/// Provides API endpoints for querying, updating, and deleting client entities.
/// </summary>
public class ClientsController(ISender sender) : BaseAPIController(sender)
{
  /// <summary>
  /// Retrieves a paginated list of client summaries based on filter criteria.
  /// </summary>
  /// <param name="requestParameters">Filtering and pagination options.</param>
  /// <param name="cancellationToken">Token for request cancellation.</param>
  /// <returns>
  /// An <see cref="IActionResult"/> containing a paged list of <see cref="ShortClientDTO"/>s.
  /// Pagination metadata is returned in the <c>X-Pagination</c> response header.
  /// </returns>
  [HttpGet]
  [ProducesResponseType(typeof(IEnumerable<ShortClientDTO>), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetClientsByFilter([FromQuery] RequestParameters requestParameters,
                                                      CancellationToken cancellationToken)
  {
    var result = await Sender.Send(new GetClientsByFilterQuery(requestParameters), cancellationToken);
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
  /// Retrieves a paginated list of clients along with related entities based on filter criteria.
  /// </summary>
  /// <param name="requestParameters">Filtering and pagination options.</param>
  /// <param name="cancellationToken">Token for request cancellation.</param>
  /// <returns>
  /// An <see cref="IActionResult"/> containing a paged list of <see cref="DetailClientDTO"/>s.
  /// Pagination metadata is returned in the <c>X-Pagination</c> response header.
  /// </returns>
  [HttpGet]
  [ProducesResponseType(typeof(IEnumerable<DetailClientDTO>), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetClientsWithChildrenByFilter([FromQuery] RequestParameters requestParameters,
                                                                  CancellationToken cancellationToken)
  {
    var result = await Sender.Send(new GetClientsWithChildrenByFilterQuery(requestParameters), cancellationToken);
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
  /// Retrieves a single client by its unique identifier.
  /// </summary>
  /// <param name="id">The unique client ID.</param>
  /// <param name="cancellationToken">Token for request cancellation.</param>
  /// <returns>
  /// An <see cref="IActionResult"/> containing a <see cref="DetailClientDTO"/> if found,
  /// or an <see cref="ApiErrorResponse"/> with status <c>404</c> if not.
  /// </returns>
  [HttpGet("{id}")]
  [ProducesResponseType(typeof(DetailClientDTO), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetClientById(Guid id, CancellationToken cancellationToken)
    => Ok(await Sender.Send(new GetClientByIdQuery(id), cancellationToken));

  /// <summary>
  /// Retrieves a single client along with related entities using filter-based criteria.
  /// </summary>
  /// <param name="requestParameters">Filtering parameters used to locate the client.</param>
  /// <param name="cancellationToken">Token for request cancellation.</param>
  /// <returns>
  /// An <see cref="IActionResult"/> containing a <see cref="DetailClientDTO"/> if found,
  /// or an <see cref="ApiErrorResponse"/> with status <c>404</c> if not.
  /// </returns>
  [HttpGet]
  [ProducesResponseType(typeof(DetailClientDTO), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetClientWithChildrenByFilter([FromQuery] RequestParameters requestParameters,
                                                                 CancellationToken cancellationToken)
    => Ok(await Sender.Send(new GetClientWithChildrenByFilterQuery(requestParameters), cancellationToken));

  /// <summary>
  /// Updates the local database with client data from an external source.
  /// </summary>
  /// <param name="cancellationToken">Token for request cancellation.</param>
  /// <returns><see cref="StatusCodes.Status200OK"/> on success, or <see cref="ApiErrorResponse"/> if update fails.</returns>
  [HttpPut]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> UpdateClientsLocalDb(CancellationToken cancellationToken)
    => Ok(await Sender.Send(new UpdateClientsLocalDbCommand(), cancellationToken));

  /// <summary>
  /// Deletes local client data from the database.
  /// </summary>
  /// <param name="cancellationToken">Token for request cancellation.</param>
  /// <returns><see cref="StatusCodes.Status200OK"/> upon successful deletion.</returns>
  [HttpDelete]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public async Task<IActionResult> DeleteClientsLocalDb(CancellationToken cancellationToken)
    => Ok(await Sender.Send(new DeleteClientsLocalDbCommand(), cancellationToken));
}

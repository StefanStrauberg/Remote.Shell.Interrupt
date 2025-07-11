namespace Remote.Shell.Interrupt.Storehouse.API.Controllers;

/// <summary>
/// Handles API requests related to SPRVlan entities.
/// </summary>
public class SPRVlansController(ISender sender) : BaseAPIController(sender)
{
  /// <summary>
  /// Retrieves a filtered and paginated list of SPRVlan records based on the provided query parameters.
  /// </summary>
  /// <param name="requestParameters">Filtering and pagination options passed via query string.</param>
  /// <param name="cancellationToken">Token used to cancel the request if necessary.</param>
  /// <returns>
  /// An <see cref="IActionResult"/> containing a paged collection of <see cref="SPRVlanDTO"/> objects.
  /// Adds pagination metadata to the response headers under the <c>X-Pagination</c> key.
  /// </returns>
  [HttpGet]
  [ProducesResponseType(typeof(IEnumerable<SPRVlanDTO>), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetSPRVlansByFilter([FromQuery] RequestParameters requestParameters,
                                                       CancellationToken cancellationToken)
  {
    var result = await Sender.Send(new GetSPRVlansByFilterQuery(requestParameters), cancellationToken);
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
}

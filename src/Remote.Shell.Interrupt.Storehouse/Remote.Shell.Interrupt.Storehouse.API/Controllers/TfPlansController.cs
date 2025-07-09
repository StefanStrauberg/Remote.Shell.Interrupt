namespace Remote.Shell.Interrupt.Storehouse.API.Controllers;

/// <summary>
/// Handles API requests related to tariff plans.
/// </summary>
public class TfPlansController : BaseAPIController
{
  /// <summary>
  /// Retrieves tariff plan records based on the specified filter and pagination parameters.
  /// </summary>
  /// <param name="requestParameters">Contains filtering and pagination settings passed via query string.</param>
  /// <param name="cancellationToken">Token used to cancel the request if needed.</param>
  /// <returns>
  /// An <see cref="IActionResult"/> containing a paged list of <see cref="TfPlanDTO"/> objects.
  /// Also appends pagination metadata to the response headers under <c>X-Pagination</c>.
  /// </returns>
  [HttpGet]
  [ProducesResponseType(typeof(IEnumerable<TfPlanDTO>), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetTfPlansByFilter([FromQuery] RequestParameters requestParameters,
                                                      CancellationToken cancellationToken)
  {
    var result = await Sender.Send(new GetTfPlansByFilterQuery(requestParameters),
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
    Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));
    return Ok(result);
  }
}

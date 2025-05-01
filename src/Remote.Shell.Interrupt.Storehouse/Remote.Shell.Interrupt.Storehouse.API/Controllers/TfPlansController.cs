namespace Remote.Shell.Interrupt.Storehouse.API.Controllers;

public class TfPlansController : BaseAPIController
{
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
    Response.Headers
            .Append("X-Pagination",
                    JsonSerializer.Serialize(metadata));
    return Ok(result);
  }  
}

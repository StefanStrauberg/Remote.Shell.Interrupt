namespace Remote.Shell.Interrupt.Storehouse.API.Controllers;

public class SPRVlansController : BaseAPIController
{
  [HttpGet]
  [ProducesResponseType(typeof(IEnumerable<SPRVlanDTO>), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetSPRVlans([FromQuery] RequestParameters requestParameters,
                                               CancellationToken cancellationToken)
  {
      var result = await Sender.Send(new GetSPRVlansQuery(requestParameters),
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

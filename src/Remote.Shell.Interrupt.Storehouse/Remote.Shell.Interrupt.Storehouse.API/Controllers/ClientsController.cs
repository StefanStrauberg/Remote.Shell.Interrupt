namespace Remote.Shell.Interrupt.Storehouse.API.Controllers;

public class ClientsController : BaseAPIController
{
  [HttpGet]
  [ProducesResponseType(typeof(IEnumerable<ShortClientDTO>), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetClientsByFilter([FromQuery] RequestParameters requestParameters,
                                                      CancellationToken cancellationToken)
  {
    var result = await Sender.Send(new GetClientsByFilterQuery(requestParameters),
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

  [HttpGet]
  [ProducesResponseType(typeof(IEnumerable<DetailClientDTO>), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetClientsWithChildrenByFilter([FromQuery] RequestParameters requestParameters,
                                                                  CancellationToken cancellationToken)
  {
    var result = await Sender.Send(new GetClientsWithChildrenByFilterQuery(requestParameters),
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
  [ProducesResponseType(typeof(DetailClientDTO), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetClientById(Guid id,
                                                 CancellationToken cancellationToken)
    => Ok(await Sender.Send(new GetClientByIdQuery(id),
                            cancellationToken));

  [HttpGet]
  [ProducesResponseType(typeof(DetailClientDTO), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetClientWithChildrenByFilter([FromQuery] RequestParameters requestParameters,
                                                                 CancellationToken cancellationToken)
    => Ok(await Sender.Send(new GetClientWithChildrenByFilterQuery(requestParameters),
                            cancellationToken));

  [HttpPut]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> UpdateClientsLocalDb(CancellationToken cancellationToken)
    => Ok(await Sender.Send(new UpdateClientsLocalDbCommand(),
                            cancellationToken));

  [HttpDelete]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public async Task<IActionResult> DeleteClientsLocalDb(CancellationToken cancellationToken)
    => Ok(await Sender.Send(new DeleteClientsLocalDbCommand(),
                            cancellationToken));
}

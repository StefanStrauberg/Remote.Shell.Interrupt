namespace Remote.Shell.Interrupt.Storehouse.API.Controllers;

public class ClientsController : BaseAPIController
{
  [HttpGet]
  [ProducesResponseType(typeof(IEnumerable<ClientDTO>), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetShortClients([FromQuery] RequestParameters requestParameters,
                                                   CancellationToken cancellationToken)
  {
    var result = await Sender.Send(new GetAllShortClientsQuery(requestParameters),
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
  [ProducesResponseType(typeof(IEnumerable<ClientDTO>), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetClientsWithChildrens([FromQuery] RequestParameters requestParameters,
                                                           CancellationToken cancellationToken)
  {
    var result = await Sender.Send(new GetAllClientsWithChildrensQuery(requestParameters),
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
    => Ok(await Sender.Send(new GetClientByIdQuery(id), cancellationToken));

  [HttpGet("{vlanTag}")]
  [ProducesResponseType(typeof(IEnumerable<ClientDTO>), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetClientsByVlanTag(int vlanTag,
                                                       CancellationToken cancellationToken)
    => Ok(await Sender.Send(new GetClientsByVlanTagQuery(vlanTag),
                            cancellationToken));

  [HttpPut]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> UpdateClients(CancellationToken cancellationToken)
    => Ok(await Sender.Send(new UpdateClientsLocalDbCommand(),
                            cancellationToken));
}

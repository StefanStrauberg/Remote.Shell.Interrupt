namespace Remote.Shell.Interrupt.Storehouse.API.Controllers;

public class GatesController : BaseAPIController
{
  [HttpGet]
  [ProducesResponseType(typeof(IEnumerable<ClientDTO>), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetGates([FromQuery] RequestParameters requestParameters,
                                            CancellationToken cancellationToken)
  {
    var result = await Sender.Send(new GetGatesQuery(requestParameters),
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
  [ProducesResponseType(typeof(GateDTO), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetGateById(Guid id,
                                               CancellationToken cancellationToken)
    => Ok(await Sender.Send(new GetGateByIdQuery(id),
                            cancellationToken));

  [HttpPost]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public async Task<IActionResult> CreateGate([FromBody] CreateGateDTO createGateDTO,
                                              CancellationToken cancellationToken)
    => Ok(await Sender.Send(new CreateGateCommand(createGateDTO),
                            cancellationToken));

  [HttpDelete("{Id}")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> DeleteGateById(Guid Id, CancellationToken cancellationToken)
    => Ok(await Sender.Send(new DeleteGateCommand(Id),
                            cancellationToken));

  [HttpPut]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> UpdateGate([FromBody] UpdateGateDTO updateGateDTO,
                                              CancellationToken cancellationToken)
    => Ok(await Sender.Send(new UpdateGateCommand(updateGateDTO),
                            cancellationToken));
}

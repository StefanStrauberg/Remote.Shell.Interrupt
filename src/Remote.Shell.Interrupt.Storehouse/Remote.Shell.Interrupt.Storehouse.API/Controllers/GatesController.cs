namespace Remote.Shell.Interrupt.Storehouse.API.Controllers;

public class GatesController(ISender sender) : BaseAPIController
{
  readonly ISender _sender = sender
    ?? throw new ArgumentNullException(nameof(sender));

  [HttpGet]
  [ProducesResponseType(typeof(IEnumerable<ClientCODDTO>), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetGates(CancellationToken cancellationToken)
    => Ok(await _sender.Send(new GetGatesQuery(),
                             cancellationToken));

  [HttpGet("{id}")]
  [ProducesResponseType(typeof(GateDTO), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetGateById(Guid id,
                                               CancellationToken cancellationToken)
    => Ok(await _sender.Send(new GetGateByIdQuery(id),
                             cancellationToken));

  [HttpPost]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public async Task<IActionResult> CreateGate([FromBody] CreateGateDTO createGateDTO,
                                              CancellationToken cancellationToken)
    => Ok(await _sender.Send(new CreateGateCommand(createGateDTO),
                             cancellationToken));

  [HttpDelete("{Id}")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> DeleteGateById(Guid Id, CancellationToken cancellationToken)
    => Ok(await _sender.Send(new DeleteGateCommand(Id),
                             cancellationToken));

  [HttpPut]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> UpdateGate([FromBody] UpdateGateDTO updateGateDTO,
                                              CancellationToken cancellationToken)
    => Ok(await _sender.Send(new UpdateGateCommand(updateGateDTO),
                             cancellationToken));
}

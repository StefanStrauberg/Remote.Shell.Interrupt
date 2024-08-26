namespace Remote.Shell.Interrupt.Storehouse.API.Controllers;

public class AssignmentsController(ISender sender) : BaseAPIController
{
  readonly ISender _sender = sender
      ?? throw new ArgumentNullException(nameof(sender));

  [HttpGet]
  [ProducesResponseType(typeof(IEnumerable<AssignmentDTO>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetAssignments(CancellationToken cancellationToken)
    => Ok(await _sender.Send(new GetAssignmentsQuery(),
                             cancellationToken));

  [HttpGet("{id}")]
  [ProducesResponseType(typeof(AssignmentDTO), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetAssignmentById(Guid id,
                                                     CancellationToken cancellationToken)
  => Ok(await _sender.Send(new GetAssignmentByExpressionQuery(x => x.Id == id),
                           cancellationToken));

  [HttpPost]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public async Task<IActionResult> CreateAssignment([FromBody] CreateAssignmentDTO createAssignmentDTO,
                                                    CancellationToken cancellationToken)
    => Ok(await _sender.Send(new CreateAssignmentCommand(createAssignmentDTO),
                             cancellationToken));

  [HttpPut("{id}")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> UpdateAssignment(Guid id,
                                                    [FromBody] UpdateAssignmentDTO updateAssignmentDTO,
                                                    CancellationToken cancellationToken)
    => Ok(await _sender.Send(new UpdateAssignmentCommand(id,
                                                         updateAssignmentDTO),
                             cancellationToken));

  [HttpDelete("{id}")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> DeleteAssignmentById(Guid id,
                                                        CancellationToken cancellationToken)
    => Ok(await _sender.Send(new DeleteAssignmentByExpressionCommand(x => x.Id == id),
                             cancellationToken));
}

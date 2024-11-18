namespace Remote.Shell.Interrupt.Storehouse.API.Controllers;

public class AssignmentsController(ISender sender) : BaseAPIController
{
  readonly ISender _sender = sender
      ?? throw new ArgumentNullException(nameof(sender));

  [HttpGet]
  [ProducesResponseType(typeof(IEnumerable<AssignmentDTO>), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetAssignments(CancellationToken cancellationToken)
    => Ok(await _sender.Send(new GetAssignmentsQuery(),
                             cancellationToken));

  [HttpGet("{id}")]
  [ProducesResponseType(typeof(AssignmentDetailDTO), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetAssignmentById(Guid id,
                                                     CancellationToken cancellationToken)
  => Ok(await _sender.Send(new GetAssignmentByIdQuery(id),
                           cancellationToken));

  [HttpPost]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public async Task<IActionResult> CreateAssignment([FromBody] CreateAssignmentDTO createAssignmentDTO,
                                                    CancellationToken cancellationToken)
    => Ok(await _sender.Send(new CreateAssignmentCommand(createAssignmentDTO),
                             cancellationToken));

  [HttpPut]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> UpdateAssignment([FromBody] UpdateAssignmentDTO updateAssignmentDTO,
                                                    CancellationToken cancellationToken)
    => Ok(await _sender.Send(new UpdateAssignmentCommand(updateAssignmentDTO),
                             cancellationToken));

  [HttpDelete("{id}")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> DeleteAssignmentById(Guid id,
                                                        CancellationToken cancellationToken)
    => Ok(await _sender.Send(new DeleteAssignmentByIdCommand(id),
                             cancellationToken));
}

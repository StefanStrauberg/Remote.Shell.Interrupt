namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Assignments;

public class UpdateAssignmentDTO
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public TypeOfRequest TypeOfRequest { get; set; }
  public string TargetFieldName { get; set; } = string.Empty;
  public string OID { get; set; } = string.Empty;
};

public record UpdateAssignmentCommand(Guid Id,
                                      UpdateAssignmentDTO UpdateAssignmentDTO) : ICommand;

internal class UpdateAssignmentCommandHandler(IAssignmentRepository assignmentRepository)
  : ICommandHandler<UpdateAssignmentCommand, Unit>
{
  readonly IAssignmentRepository _assignmentRepository = assignmentRepository
    ?? throw new ArgumentNullException(nameof(assignmentRepository));

  async Task<Unit> IRequestHandler<UpdateAssignmentCommand, Unit>.Handle(UpdateAssignmentCommand request,
                                                                         CancellationToken cancellationToken)
  {
    var existingUpdatingAssignment = await _assignmentRepository.ExistsAsync(filterExpression: x => x.Id == request.Id,
                                                                             cancellationToken: cancellationToken);

    if (!existingUpdatingAssignment)
      throw new EntityNotFoundException(request.Id.ToString());

    request.UpdateAssignmentDTO.Id = request.Id;

    var updatingAssignment = request.UpdateAssignmentDTO
                                    .Adapt<Assignment>();

    await _assignmentRepository.ReplaceOneAsync(filterExpression: x => x.Id == request.Id,
                                                document: updatingAssignment,
                                                cancellationToken: cancellationToken);
    return Unit.Value;
  }
}

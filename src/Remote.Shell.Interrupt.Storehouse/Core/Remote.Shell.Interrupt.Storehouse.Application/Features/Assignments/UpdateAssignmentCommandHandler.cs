namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Assignments;

public record UpdateAssignmentCommand(Guid Id,
                                      string Name,
                                      TypeOfRequest TypeOfRequest,
                                      string TargetFieldName,
                                      string OID) : ICommand;

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

    var updatingAssignment = request.Adapt<Assignment>();

    await _assignmentRepository.ReplaceOneAsync(filterExpression: x => x.Id == request.Id,
                                                document: updatingAssignment,
                                                cancellationToken: cancellationToken);
    return Unit.Value;
  }
}

namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Assignments;

public record CreateAssignmentCommand(string Name,
                                      TypeOfRequest TypeOfRequest,
                                      string TargetFieldName,
                                      string OID) : ICommand;

internal class CreateAssignmentCommandHandler(IAssignmentRepository assignmentRepository)
  : ICommandHandler<CreateAssignmentCommand, Unit>
{
  readonly IAssignmentRepository _assignmentRepository = assignmentRepository
    ?? throw new ArgumentNullException(nameof(assignmentRepository));

  async Task<Unit> IRequestHandler<CreateAssignmentCommand, Unit>.Handle(CreateAssignmentCommand request,
                                                                         CancellationToken cancellationToken)
  {
    var existingAssignment = await _assignmentRepository.ExistsAsync(filterExpression: x => x.TargetFieldName == request.Name,
                                                                     cancellationToken: cancellationToken);

    if (existingAssignment)
      throw new EntityAlreadyExists(request.Name);

    var assignment = request.Adapt<Assignment>();

    await _assignmentRepository.InsertOneAsync(document: assignment,
                                               cancellationToken: cancellationToken);

    return Unit.Value;
  }
}

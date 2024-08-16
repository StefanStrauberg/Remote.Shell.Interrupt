namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Assignments;

public record DeleteAssignmentCommand(Guid Id) : ICommand;

internal class DeleteAssignmentCommandHandler(IAssignmentRepository assignmentRepository)
  : ICommandHandler<DeleteAssignmentCommand, Unit>
{
  readonly IAssignmentRepository _assignmentRepository = assignmentRepository
    ?? throw new ArgumentNullException(nameof(assignmentRepository));

  async Task<Unit> IRequestHandler<DeleteAssignmentCommand, Unit>.Handle(DeleteAssignmentCommand request,
                                                                         CancellationToken cancellationToken)
  {
    var existingAssignment = await _assignmentRepository.ExistsAsync(x => x.Id == request.Id,
                                                                     cancellationToken);

    if (!existingAssignment)
      throw new EntityNotFoundException(request.Id.ToString());

    await _assignmentRepository.DeleteOneAsync(x => x.Id == request.Id,
                                               cancellationToken);
    return Unit.Value;
  }
}

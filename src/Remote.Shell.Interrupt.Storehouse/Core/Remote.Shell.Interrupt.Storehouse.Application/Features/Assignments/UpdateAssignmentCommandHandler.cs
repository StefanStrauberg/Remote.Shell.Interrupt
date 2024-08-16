namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Assignments;

public record UpdateAssignmentCommand(Guid Id,
                                      string Name,
                                      TypeOfRequest TypeOfRequest,
                                      string TargetFieldName,
                                      string OID) : ICommand;

internal class UpdateAssignmentCommandHandler(IAssignmentRepository oidTargetRepository)
  : ICommandHandler<UpdateAssignmentCommand, Unit>
{
  readonly IAssignmentRepository _OIDTargetRepository = oidTargetRepository
    ?? throw new ArgumentNullException(nameof(oidTargetRepository));

  async Task<Unit> IRequestHandler<UpdateAssignmentCommand, Unit>.Handle(UpdateAssignmentCommand request,
                                                                         CancellationToken cancellationToken)
  {
    var existingUpdatingAssignment = await _OIDTargetRepository.ExistsAsync(x => x.Id == request.Id,
                                                                            cancellationToken);

    if (!existingUpdatingAssignment)
      throw new EntityNotFoundException(request.Id.ToString());

    var updatingAssignment = request.Adapt<Assignment>();

    await _OIDTargetRepository.ReplaceOneAsync(x => x.Id == request.Id,
                                               updatingAssignment,
                                               cancellationToken);
    return Unit.Value;
  }
}

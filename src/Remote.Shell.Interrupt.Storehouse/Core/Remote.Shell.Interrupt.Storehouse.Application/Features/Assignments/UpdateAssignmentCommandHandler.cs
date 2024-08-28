namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Assignments;

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
    var assignmentId = request.Id;
    var dto = request.UpdateAssignmentDTO;

    // Check existing an Assignment with a specific ID
    Expression<Func<Assignment, bool>> filterByID = x => x.Id == request.Id;

    var existingUpdatingAssignmentById = await _assignmentRepository.ExistsAsync(filterExpression: filterByID,
                                                                                 cancellationToken: cancellationToken);

    if (!existingUpdatingAssignmentById)
      throw new EntityNotFoundException(new ExpressionToStringConverter<Assignment>().Convert(filterByID));

    // Check existing an Assignment with a specific Name but with an another ID
    Expression<Func<Assignment, bool>> filterUniqueName = x => x.Name == request.UpdateAssignmentDTO.Name &&
                                                          x.Id != request.Id;

    var existingUpdatingAssignmentByName = await _assignmentRepository.ExistsAsync(filterExpression: filterUniqueName,
                                                                                   cancellationToken: cancellationToken);

    if (existingUpdatingAssignmentByName)
      throw new EntityAlreadyExists(new ExpressionToStringConverter<Assignment>().Convert(filterUniqueName));

    // Update the Assignment
    var updatingAssignment = request.UpdateAssignmentDTO
                                    .Adapt<Assignment>();

    updatingAssignment.Id = request.Id;
    updatingAssignment.Modified = DateTime.Now;

    await _assignmentRepository.ReplaceOneAsync(filterExpression: x => x.Id == request.Id,
                                                document: updatingAssignment,
                                                cancellationToken: cancellationToken);
    return Unit.Value;
  }
}

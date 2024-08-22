namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Assignments;

public record DeleteAssignmentByExpressionCommand(Expression<Func<Assignment, bool>> FilterExpression)
  : ICommand;

internal class DeleteAssignmentByExpressionCommandHandler(IAssignmentRepository assignmentRepository)
  : ICommandHandler<DeleteAssignmentByExpressionCommand, Unit>
{
  readonly IAssignmentRepository _assignmentRepository = assignmentRepository
    ?? throw new ArgumentNullException(nameof(assignmentRepository));

  async Task<Unit> IRequestHandler<DeleteAssignmentByExpressionCommand, Unit>.Handle(DeleteAssignmentByExpressionCommand request,
                                                                                     CancellationToken cancellationToken)
  {
    var existingAssignment = await _assignmentRepository.ExistsAsync(filterExpression: request.FilterExpression,
                                                                     cancellationToken: cancellationToken);

    if (!existingAssignment)
      throw new EntityNotFoundException(request.ToString());

    await _assignmentRepository.DeleteOneAsync(filterExpression: request.FilterExpression,
                                               cancellationToken: cancellationToken);
    return Unit.Value;
  }
}

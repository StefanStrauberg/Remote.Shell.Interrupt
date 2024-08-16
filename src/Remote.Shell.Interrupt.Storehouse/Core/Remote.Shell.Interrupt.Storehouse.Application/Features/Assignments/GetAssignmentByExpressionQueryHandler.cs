namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Assignments;

public record GetAssignmentByExpressionQuery(Expression<Func<Assignment, bool>> FilterExpression)
  : IQuery<Assignment>;

internal class GetAssignmentByExpressionQueryHandler(IAssignmentRepository assignmentRepository)
  : IQueryHandler<GetAssignmentByExpressionQuery, Assignment>
{
  readonly IAssignmentRepository _assignmentRepository = assignmentRepository
    ?? throw new ArgumentNullException(nameof(assignmentRepository));

  async Task<Assignment> IRequestHandler<GetAssignmentByExpressionQuery, Assignment>.Handle(GetAssignmentByExpressionQuery request,
                                                                                            CancellationToken cancellationToken)
  {
    var assignment = await _assignmentRepository.FindOneAsync(request.FilterExpression, cancellationToken)
      ?? throw new EntityNotFoundException(request.FilterExpression.Name!);

    return assignment;
  }
}

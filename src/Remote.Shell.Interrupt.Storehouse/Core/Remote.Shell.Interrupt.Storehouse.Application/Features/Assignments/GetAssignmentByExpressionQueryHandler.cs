namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Assignments;

public record GetAssignmentByExpressionQuery(Expression<Func<Assignment, bool>> FilterExpression)
  : IQuery<AssignmentDTO>;

internal class GetAssignmentByExpressionQueryHandler(IAssignmentRepository assignmentRepository)
  : IQueryHandler<GetAssignmentByExpressionQuery, AssignmentDTO>
{
  readonly IAssignmentRepository _assignmentRepository = assignmentRepository
    ?? throw new ArgumentNullException(nameof(assignmentRepository));

  async Task<AssignmentDTO> IRequestHandler<GetAssignmentByExpressionQuery, AssignmentDTO>.Handle(GetAssignmentByExpressionQuery request,
                                                                                                  CancellationToken cancellationToken)
  {
    var assignment = await _assignmentRepository.FindOneAsync(filterExpression: request.FilterExpression,
                                                              cancellationToken: cancellationToken)
      ?? throw new EntityNotFoundException(request.FilterExpression.Name!);
    var assignmentDTO = assignment.Adapt<AssignmentDTO>();

    return assignmentDTO;
  }
}

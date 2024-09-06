namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Assignments.Queries.GetByExpression;

public record GetAssignmentByExpressionQuery(Expression<Func<Assignment, bool>> FilterExpression)
  : IQuery<AssignmentDetailDTO>;

internal class GetAssignmentByExpressionQueryHandler(IAssignmentRepository assignmentRepository,
                                                     IMapper mapper)
  : IQueryHandler<GetAssignmentByExpressionQuery, AssignmentDetailDTO>
{
  readonly IAssignmentRepository _assignmentRepository = assignmentRepository
    ?? throw new ArgumentNullException(nameof(assignmentRepository));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<AssignmentDetailDTO> IRequestHandler<GetAssignmentByExpressionQuery, AssignmentDetailDTO>.Handle(GetAssignmentByExpressionQuery request,
                                                                                                              CancellationToken cancellationToken)
  {
    var assignment = await _assignmentRepository.FindOneAsync(filterExpression: request.FilterExpression,
                                                              cancellationToken: cancellationToken)
      ?? throw new EntityNotFoundException(new ExpressionToStringConverter<Assignment>().Convert(request.FilterExpression));
    var assignmentDetailDTO = _mapper.Map<AssignmentDetailDTO>(assignment);

    return assignmentDetailDTO;
  }
}

namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Assignments.Queries.GetByExpression;

public record GetAssignmentByExpressionQuery(Expression<Func<Assignment, bool>> FilterExpression)
  : IQuery<AssignmentDetailDTO>;

internal class GetAssignmentByExpressionQueryHandler(IUnitOfWork unitOfWork,
                                                     IMapper mapper)
  : IQueryHandler<GetAssignmentByExpressionQuery, AssignmentDetailDTO>
{
  readonly IUnitOfWork _uniOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<AssignmentDetailDTO> IRequestHandler<GetAssignmentByExpressionQuery, AssignmentDetailDTO>.Handle(GetAssignmentByExpressionQuery request,
                                                                                                              CancellationToken cancellationToken)
  {
    var assignment = await _uniOfWork.Assignments
                                     .FirstAsync(request.FilterExpression, cancellationToken)
      ?? throw new EntityNotFoundException(new ExpressionToStringConverter<Assignment>().Convert(request.FilterExpression));

    var assignmentDetailDTO = _mapper.Map<AssignmentDetailDTO>(assignment);

    return assignmentDetailDTO;
  }
}

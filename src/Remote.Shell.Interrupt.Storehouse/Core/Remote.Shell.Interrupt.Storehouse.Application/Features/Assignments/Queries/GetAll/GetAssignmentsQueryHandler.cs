namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Assignments.Queries.GetAll;

public record GetAssignmentsQuery() : IQuery<IEnumerable<AssignmentDTO>>;

internal class GetAssignmentsQueryHandler(IUnitOfWork unitOfWork,
                                          IMapper mapper)
  : IQueryHandler<GetAssignmentsQuery, IEnumerable<AssignmentDTO>>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<IEnumerable<AssignmentDTO>> IRequestHandler<GetAssignmentsQuery, IEnumerable<AssignmentDTO>>.Handle(GetAssignmentsQuery request,
                                                                                                                 CancellationToken cancellationToken)
  {
    // Получаем все назначения из репозитория
    var assignments = await _unitOfWork.Assignments
                                       .GetAllAsync(cancellationToken);

    // Преобразуем доменные модели в DTO
    var assignmentsDTOs = _mapper.Map<IEnumerable<AssignmentDTO>>(assignments);

    // Возвращаем список DTO
    return assignmentsDTOs;
  }
}

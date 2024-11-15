namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Assignments.Queries.GetByExpression;

public record GetAssignmentByIdQuery(Guid Id)
  : IQuery<AssignmentDetailDTO>;

internal class GetAssignmentByIdQueryHandler(IUnitOfWork unitOfWork,
                                                     IMapper mapper)
  : IQueryHandler<GetAssignmentByIdQuery, AssignmentDetailDTO>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<AssignmentDetailDTO> IRequestHandler<GetAssignmentByIdQuery, AssignmentDetailDTO>.Handle(GetAssignmentByIdQuery request,
                                                                                                      CancellationToken cancellationToken)
  {
    // Проверка существования назначения с данным ID
    var existingAssignment = await _unitOfWork.Assignments
                                              .AnyByIdAsync(request.Id,
                                                            cancellationToken);

    // Если назначение не найдено — исключение
    if (!existingAssignment)
      throw new EntityNotFoundById(typeof(Assignment),
                                   request.Id.ToString());

    // Получаем назначение из базы данных
    var assignment = await _unitOfWork.Assignments
                                      .FirstByIdAsync(request.Id,
                                                      cancellationToken);

    // Преобразуем назначение в DTO
    var assignmentDetailDTO = _mapper.Map<AssignmentDetailDTO>(assignment);

    // Возвращаем DTO
    return assignmentDetailDTO;
  }
}

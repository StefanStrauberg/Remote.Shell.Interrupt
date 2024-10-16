namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Assignments.Commands.Create;

internal class CreateAssignmentCommandHandler(IUnitOfWork unitOfWork,
                                              IMapper mapper)
  : ICommandHandler<CreateAssignmentCommand, Unit>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<Unit> IRequestHandler<CreateAssignmentCommand, Unit>.Handle(CreateAssignmentCommand request,
                                                                         CancellationToken cancellationToken)
  {
    // Создание фильтра для проверки уникальности имени назначения
    Expression<Func<Assignment, bool>> filter = x => x.Name == request.CreateAssignmentDTO
                                                                      .Name;
    // Проверка существует ли уже назначение с таким именем
    var existingAssignment = await _unitOfWork.Assignments
                                              .AnyAsync(filter, cancellationToken);

    // Если назначение с таким именем уже существует, выбрасываем исключение
    if (existingAssignment)
      throw new EntityAlreadyExists(new ExpressionToStringConverter<Assignment>().Convert(filter));

    // Преобразование DTO в доменную модель назначения
    var assignment = _mapper.Map<Assignment>(request.CreateAssignmentDTO);

    // Вставка нового назначения в репозиторий
    _unitOfWork.Assignments
               .InsertOne(assignment);

    await _unitOfWork.CompleteAsync(cancellationToken);

    // Возврат успешного завершения операции
    return Unit.Value;
  }
}

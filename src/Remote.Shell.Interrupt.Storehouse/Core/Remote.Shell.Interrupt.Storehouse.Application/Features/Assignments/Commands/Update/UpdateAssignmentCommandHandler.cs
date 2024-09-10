namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Assignments.Commands.Update;

internal class UpdateAssignmentCommandHandler(IAssignmentRepository assignmentRepository,
                                              IMapper mapper)
  : ICommandHandler<UpdateAssignmentCommand, Unit>
{
  readonly IAssignmentRepository _assignmentRepository = assignmentRepository
    ?? throw new ArgumentNullException(nameof(assignmentRepository));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<Unit> IRequestHandler<UpdateAssignmentCommand, Unit>.Handle(UpdateAssignmentCommand request,
                                                                         CancellationToken cancellationToken)
  {
    // Создание фильтра для поиска назначения по его ID
    var filterByID = (Expression<Func<Assignment, bool>>)(x => x.Id == request.UpdateAssignmentDTO.Id);

    // Проверка существует ли назначение с указанным ID
    var existingUpdatingAssignmentById = await _assignmentRepository.ExistsAsync(filterExpression: filterByID,
                                                                                 cancellationToken: cancellationToken);

    // Если назначение с таким ID не найдено, выбрасываем исключение
    if (!existingUpdatingAssignmentById)
      throw new EntityNotFoundException(new ExpressionToStringConverter<Assignment>().Convert(filterByID));

    // Создание фильтра для проверки уникальности имени назначения
    // Проверка существует ли назначение с таким же именем, но с другим ID
    var filterUniqueName = (Expression<Func<Assignment, bool>>)(x => x.Name == request.UpdateAssignmentDTO
                                                                                      .Name &&
                                                                     x.Id != request.UpdateAssignmentDTO.Id);
    var existingUpdatingAssignmentByName = await _assignmentRepository.ExistsAsync(filterExpression: filterUniqueName,
                                                                                   cancellationToken: cancellationToken);

    // Если назначение с таким именем уже существует и это не то назначение
    // которое обновляется, выбрасываем исключение
    if (existingUpdatingAssignmentByName)
      throw new EntityAlreadyExists(new ExpressionToStringConverter<Assignment>().Convert(filterUniqueName));

    // Получаем назначение для обновления
    var assignment = await _assignmentRepository.FindOneAsync(filterExpression: filterByID,
                                                              cancellationToken: cancellationToken);

    // Преобразование DTO в доменную модель назначения
    _mapper.Map(request.UpdateAssignmentDTO, assignment);

    // Обновление назначения в репозитории
    await _assignmentRepository.ReplaceOneAsync(filterExpression: x => x.Id == request.UpdateAssignmentDTO.Id,
                                                document: assignment,
                                                cancellationToken: cancellationToken);

    // Возврат успешного завершения операции
    return Unit.Value;
  }
}

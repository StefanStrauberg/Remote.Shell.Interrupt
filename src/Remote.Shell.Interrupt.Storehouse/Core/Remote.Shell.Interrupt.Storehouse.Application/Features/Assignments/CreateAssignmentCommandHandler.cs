namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Assignments;

public record CreateAssignmentCommand(CreateAssignmentDTO CreateAssignmentDTO) : ICommand;

internal class CreateAssignmentCommandHandler(IAssignmentRepository assignmentRepository)
  : ICommandHandler<CreateAssignmentCommand, Unit>
{
  readonly IAssignmentRepository _assignmentRepository = assignmentRepository
    ?? throw new ArgumentNullException(nameof(assignmentRepository));

  async Task<Unit> IRequestHandler<CreateAssignmentCommand, Unit>.Handle(CreateAssignmentCommand request,
                                                                         CancellationToken cancellationToken)
  {
    // Создание фильтра для проверки уникальности имени назначения
    Expression<Func<Assignment, bool>> filter = x => x.Name == request.CreateAssignmentDTO
                                                                      .Name;
    // Проверка существует ли уже назначение с таким именем
    var existingAssignment = await _assignmentRepository.ExistsAsync(filterExpression: filter,
                                                                     cancellationToken: cancellationToken);

    // Если назначение с таким именем уже существует, выбрасываем исключение
    if (existingAssignment)
      throw new EntityAlreadyExists(new ExpressionToStringConverter<Assignment>().Convert(filter));

    // Преобразование DTO в доменную модель назначения
    var assignment = request.CreateAssignmentDTO
                            .Adapt<Assignment>();

    // Установка даты создания и последнего изменения
    assignment.Created = DateTime.Now;
    assignment.Modified = DateTime.Now;

    // Вставка нового назначения в репозиторий
    await _assignmentRepository.InsertOneAsync(document: assignment,
                                               cancellationToken: cancellationToken);

    // Возврат успешного завершения операции
    return Unit.Value;
  }
}

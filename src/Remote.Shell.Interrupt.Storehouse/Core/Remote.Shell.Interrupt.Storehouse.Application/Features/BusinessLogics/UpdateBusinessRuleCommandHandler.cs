namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics;

public record UpdateBusinessRuleCommand(Guid Id,
                                        UpdateBusinessRuleDTO UpdateBusinessRule)
  : ICommand;

internal class UpdateBusinessRuleCommandHandler(IBusinessRuleRepository businessRuleRepository,
                                                IAssignmentRepository assignmentRepository)
  : ICommandHandler<UpdateBusinessRuleCommand, Unit>
{
  readonly IBusinessRuleRepository _businessRuleRepository = businessRuleRepository
    ?? throw new ArgumentNullException(nameof(businessRuleRepository));
  readonly IAssignmentRepository _assignmentRepository = assignmentRepository
    ?? throw new ArgumentNullException(nameof(assignmentRepository));

  async Task<Unit> IRequestHandler<UpdateBusinessRuleCommand, Unit>.Handle(UpdateBusinessRuleCommand request,
                                                                           CancellationToken cancellationToken)
  {
    // Создание фильтра для поиска бизнес-правила по его ID
    Expression<Func<BusinessRule, bool>> filter = x => x.Id == request.Id;

    // Проверка существует ли бизнес-правило с указанным ID
    var existingUpdatingBusinessRule = await _businessRuleRepository.ExistsAsync(filterExpression: filter,
                                                                                 cancellationToken: cancellationToken);

    // Если бизнес-правило не найдено, выбрасываем исключение
    if (!existingUpdatingBusinessRule)
      throw new EntityNotFoundException(request.Id.ToString());

    // Проверка на существование назначения по ID для бизнес-правила
    // Создание фильтра для проверки наличия назначения по указанному ID
    if (request.UpdateBusinessRule.AssignmentId != null)
    {
      var assignmentFilter = (Expression<Func<Assignment, bool>>)(x => x.Id == request.UpdateBusinessRule
                                                                                      .AssignmentId);
      // Проверка существует ли назначение с указанным ID
      var existingAssignment = await _assignmentRepository.ExistsAsync(filterExpression: assignmentFilter,
                                                                       cancellationToken: cancellationToken);
      // Если назначение не найдено, выбрасываем исключение
      if (!existingAssignment)
        throw new EntityNotFoundException(new ExpressionToStringConverter<Assignment>().Convert(assignmentFilter));
    }

    // Преобразование команды в сущность бизнес-правила
    var updatingBusinessRule = request.Adapt<BusinessRule>();
    // Поиск оригинального бизнес-правила для сохранения его родительских и дочерних узлов
    var originalBusinessRule = await _businessRuleRepository.FindOneAsync(filterExpression: filter,
                                                                          cancellationToken: cancellationToken);

    // Установка ID и даты последнего изменения для обновляемого бизнес-правила
    updatingBusinessRule.Id = request.Id;
    updatingBusinessRule.Modified = DateTime.UtcNow;
    // Сохранение родительского ID, время создания и списка дочерних узлов оригинального бизнес-правила
    updatingBusinessRule.Created = originalBusinessRule!.Created;
    updatingBusinessRule.ParentId = originalBusinessRule.ParentId;
    updatingBusinessRule.Children = originalBusinessRule.Children;
    updatingBusinessRule.AssignmentId = request.UpdateBusinessRule.AssignmentId;

    // Обновление бизнес-правила в репозитории
    await _businessRuleRepository.ReplaceOneAsync(filterExpression: x => x.Id == request.Id,
                                                  document: updatingBusinessRule,
                                                  cancellationToken: cancellationToken);

    // Возврат успешного завершения операции
    return Unit.Value;
  }
}

namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics;

public record CreateBusinessRuleCommand(CreateBusinessRuleDTO CreateBusinessRuleDTO) : ICommand;

internal class CreateBusinessRuleCommandHandler(IBusinessRuleRepository businessRuleRepository,
                                                IAssignmentRepository assignmentRepository)
  : ICommandHandler<CreateBusinessRuleCommand, Unit>
{
  readonly IBusinessRuleRepository _businessRuleRepository = businessRuleRepository
    ?? throw new ArgumentNullException(nameof(businessRuleRepository));
  readonly IAssignmentRepository _assignmentRepository = assignmentRepository
    ?? throw new ArgumentNullException(nameof(assignmentRepository));

  async Task<Unit> IRequestHandler<CreateBusinessRuleCommand, Unit>.Handle(CreateBusinessRuleCommand request,
                                                                           CancellationToken cancellationToken)
  {
    // Проверка, указан ли ID назначения для бизнес-правила
    if (request.CreateBusinessRuleDTO.AssignmentId != null)
    {
      // Создание фильтра для проверки наличия назначения по указанному ID
      var assignmentFilter = (Expression<Func<Assignment, bool>>)(x => x.Id == request.CreateBusinessRuleDTO
                                                                                      .AssignmentId);
      // Проверка существует ли назначение с указанным ID
      var existingAssignment = await _assignmentRepository.ExistsAsync(filterExpression: assignmentFilter,
                                                                       cancellationToken: cancellationToken);
      // Если назначение не найдено, выбрасываем исключение
      if (!existingAssignment)
        throw new EntityNotFoundException(new ExpressionToStringConverter<Assignment>().Convert(assignmentFilter));
    }

    // Преобразование DTO в сущность бизнес-правила
    var addingRule = request.CreateBusinessRuleDTO
                            .Adapt<BusinessRule>();
    var isRoot = (await _businessRuleRepository.GetAllAsync(cancellationToken)).Any();

    // Установка даты создания и последнего изменения
    addingRule.Created = DateTime.UtcNow;
    addingRule.Modified = DateTime.UtcNow;

    if (!isRoot)
    {
      addingRule.IsRoot = true;
      addingRule.ParentId = null;
    }
    else
      addingRule.IsRoot = false;

    // Проверка наличия родительского бизнес-правила
    if (request.CreateBusinessRuleDTO.ParentId != null)
    {
      // Поиск родительского бизнес-правила
      var parentFilter = (Expression<Func<BusinessRule, bool>>)(x => x.Id == request.CreateBusinessRuleDTO
                                                                                    .ParentId);
      var parentRule = await _businessRuleRepository.FindOneAsync(filterExpression: parentFilter,
                                                                  cancellationToken)
        ?? throw new EntityNotFoundException(new ExpressionToStringConverter<BusinessRule>().Convert(parentFilter));

      // Вставка нового бизнес-правила в репозиторий
      await _businessRuleRepository.InsertOneAsync(document: addingRule,
                                                   cancellationToken: cancellationToken);

      // Добавление ID нового бизнес-правила в список дочерних элементов родительского бизнес-правила
      parentRule.Children.Add(addingRule.Id);

      // Обновление родительского бизнес-правила в репозитории
      await _businessRuleRepository.ReplaceOneAsync(filterExpression: parentFilter,
                                                    document: parentRule,
                                                    cancellationToken: cancellationToken);
    }
    else
    {
      // Если нет родительского бизнес-правила, просто вставляем новое бизнес-правило в репозиторий
      await _businessRuleRepository.InsertOneAsync(document: addingRule,
                                                   cancellationToken: cancellationToken);
    }

    // Возврат успешного завершения операции
    return Unit.Value;
  }
}

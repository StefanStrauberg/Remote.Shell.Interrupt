namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics.Commands.Update;

internal class UpdateBusinessRuleCommandHandler(IBusinessRuleRepository businessRuleRepository,
                                                IAssignmentRepository assignmentRepository,
                                                IMapper mapper)
  : ICommandHandler<UpdateBusinessRuleCommand, Unit>
{
  readonly IBusinessRuleRepository _businessRuleRepository = businessRuleRepository
    ?? throw new ArgumentNullException(nameof(businessRuleRepository));
  readonly IAssignmentRepository _assignmentRepository = assignmentRepository
    ?? throw new ArgumentNullException(nameof(assignmentRepository));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<Unit> IRequestHandler<UpdateBusinessRuleCommand, Unit>.Handle(UpdateBusinessRuleCommand request,
                                                                           CancellationToken cancellationToken)
  {
    // Создание фильтра для поиска бизнес-правила по указанному ID
    Expression<Func<BusinessRule, bool>> filterByID = x => x.Id == request.UpdateBusinessRule.Id;

    // Проверка существует ли бизнес-правило с указанным ID
    var existingUpdatingBusinessRule = await _businessRuleRepository.ExistsAsync(filterExpression: filterByID,
                                                                                 cancellationToken: cancellationToken);

    // Если бизнес-правило не найдено, выбрасываем исключение
    if (!existingUpdatingBusinessRule)
      throw new EntityNotFoundException(request.UpdateBusinessRule.Id.ToString());

    // Создание фильтра для проверки наличия назначения по указанному ID
    // Проверка на существование назначения по ID для бизнес-правила
    if (request.UpdateBusinessRule.AssignmentId != null)
    {
      // Создание фильтра для поиска назначения по указанному ID
      var assignmentFilter = (Expression<Func<Assignment, bool>>)(x => x.Id == request.UpdateBusinessRule
                                                                                      .AssignmentId);
      // Проверка существует ли назначение с указанным ID
      var existingAssignment = await _assignmentRepository.ExistsAsync(filterExpression: assignmentFilter,
                                                                       cancellationToken: cancellationToken);
      // Если назначение не найдено, выбрасываем исключение
      if (!existingAssignment)
        throw new EntityNotFoundException(new ExpressionToStringConverter<Assignment>().Convert(assignmentFilter));
    }

    // Получаем назначение для обновления
    var businessRule = await _businessRuleRepository.FindOneAsync(filterExpression: filterByID,
                                                                  cancellationToken: cancellationToken);

    // Преобразование DTO в доменную модель назначения
    _mapper.Map(request.UpdateBusinessRule, businessRule);

    // Установка ID и даты последнего изменения для обновляемого бизнес-правила
    businessRule!.Modified = DateTime.UtcNow;

    // Обновление бизнес-правила в репозитории
    await _businessRuleRepository.ReplaceOneAsync(filterExpression: x => x.Id == request.UpdateBusinessRule.Id,
                                                  document: businessRule,
                                                  cancellationToken: cancellationToken);

    // Возврат успешного завершения операции
    return Unit.Value;
  }
}

namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics.Commands.Create;

internal class CreateBusinessRuleCommandHandler(IBusinessRuleRepository businessRuleRepository,
                                                IAssignmentRepository assignmentRepository,
                                                IMapper mapper)
  : ICommandHandler<CreateBusinessRuleCommand, Unit>
{
  readonly IBusinessRuleRepository _businessRuleRepository = businessRuleRepository
    ?? throw new ArgumentNullException(nameof(businessRuleRepository));
  readonly IAssignmentRepository _assignmentRepository = assignmentRepository
    ?? throw new ArgumentNullException(nameof(assignmentRepository));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

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
    var addingBusinessRule = _mapper.Map<BusinessRule>(request.CreateBusinessRuleDTO);
    var isRoot = (await _businessRuleRepository.GetAllAsync(cancellationToken)).Any();

    if (!isRoot)
    {
      addingBusinessRule.IsRoot = true;
      addingBusinessRule.Parent = null;
    }
    else
      addingBusinessRule.IsRoot = false;

    // Проверка наличия родительского бизнес-правила
    if (request.CreateBusinessRuleDTO.ParentId != null)
    {
      // Поиск родительского бизнес-правила
      var parentFilter = (Expression<Func<BusinessRule, bool>>)(x => x.Id == request.CreateBusinessRuleDTO
                                                                                    .ParentId);
      var parentBusinessRule = await _businessRuleRepository.FindOneAsync(filterExpression: parentFilter,
                                                                  cancellationToken)
        ?? throw new EntityNotFoundException(new ExpressionToStringConverter<BusinessRule>().Convert(parentFilter));

      // Вставка нового бизнес-правила в репозиторий
      await _businessRuleRepository.InsertOneAsync(document: addingBusinessRule,
                                                   cancellationToken: cancellationToken);

      // Добавление ID нового бизнес-правила в список дочерних элементов родительского бизнес-правила
      parentBusinessRule.Children.Add(addingBusinessRule);

      // Обновление родительского бизнес-правила в репозитории
      await _businessRuleRepository.ReplaceOneAsync(filterExpression: parentFilter,
                                                    document: parentBusinessRule,
                                                    cancellationToken: cancellationToken);
    }
    else
    {
      // Если нет родительского бизнес-правила, просто вставляем новое бизнес-правило в репозиторий
      await _businessRuleRepository.InsertOneAsync(document: addingBusinessRule,
                                                   cancellationToken: cancellationToken);
    }
    // Возврат успешного завершения операции
    return Unit.Value;
  }
}

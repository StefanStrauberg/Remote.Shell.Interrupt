namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics.Commands.Create;

internal class CreateBusinessRuleCommandHandler(IUnitOfWork unitOfWork,
                                                IMapper mapper)
  : ICommandHandler<CreateBusinessRuleCommand, Unit>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
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
      var existingAssignment = await _unitOfWork.Assignments
                                                .AnyAsync(assignmentFilter, cancellationToken);
      // Если назначение не найдено, выбрасываем исключение
      if (!existingAssignment)
        throw new EntityNotFoundException(new ExpressionToStringConverter<Assignment>().Convert(assignmentFilter));
    }

    // Преобразование DTO в сущность бизнес-правила
    var addingBusinessRule = _mapper.Map<BusinessRule>(request.CreateBusinessRuleDTO);
    var isAny = await _unitOfWork.BusinessRules
                                 .AnyAsync(x => true, cancellationToken);

    if (!isAny)
    {
      addingBusinessRule.IsRoot = true;
      addingBusinessRule.Parent = null;
    }
    else
      addingBusinessRule.IsRoot = false;

    // Проверка наличия родительского бизнес-правила
    if (request.CreateBusinessRuleDTO.ParentId != null && isAny)
    {
      // Поиск родительского бизнес-правила
      var parentFilter = (Expression<Func<BusinessRule, bool>>)(x => x.Id == request.CreateBusinessRuleDTO
                                                                                    .ParentId);
      var parentBusinessRule = await _unitOfWork.BusinessRules
                                                .FirstAsync(parentFilter, cancellationToken)
        ?? throw new EntityNotFoundException(new ExpressionToStringConverter<BusinessRule>().Convert(parentFilter));

      // Вставка нового бизнес-правила в репозиторий
      _unitOfWork.BusinessRules
                 .InsertOne(addingBusinessRule);

      // Добавление ID нового бизнес-правила в список дочерних элементов родительского бизнес-правила
      parentBusinessRule.Children
                        .Add(addingBusinessRule);

      // Обновление родительского бизнес-правила в репозитории
      _unitOfWork.BusinessRules
                 .ReplaceOne(parentBusinessRule);
    }
    else
    {
      // Если нет родительского бизнес-правила, просто вставляем новое бизнес-правило в репозиторий
      _unitOfWork.BusinessRules
                 .InsertOne(entity: addingBusinessRule);
    }

    await _unitOfWork.CompleteAsync(cancellationToken);

    // Возврат успешного завершения операции
    return Unit.Value;
  }
}

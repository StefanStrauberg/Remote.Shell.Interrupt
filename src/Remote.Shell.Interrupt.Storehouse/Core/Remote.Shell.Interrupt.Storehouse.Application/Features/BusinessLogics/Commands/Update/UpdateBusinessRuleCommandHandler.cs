namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics.Commands.Update;

internal class UpdateBusinessRuleCommandHandler(IUnitOfWork unitOfWork,
                                                IMapper mapper)
  : ICommandHandler<UpdateBusinessRuleCommand, Unit>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<Unit> IRequestHandler<UpdateBusinessRuleCommand, Unit>.Handle(UpdateBusinessRuleCommand request,
                                                                           CancellationToken cancellationToken)
  {
    // Создание фильтра для поиска бизнес-правила по указанному ID
    Expression<Func<BusinessRule, bool>> filterByID = x => x.Id == request.UpdateBusinessRule.Id;

    // Проверка существует ли бизнес-правило с указанным ID
    var existingUpdatingBusinessRule = await _unitOfWork.BusinessRules
                                                        .AnyAsync(filterByID, cancellationToken);

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
      var existingAssignment = await _unitOfWork.Assignments
                                                .AnyAsync(assignmentFilter, cancellationToken);
      // Если назначение не найдено, выбрасываем исключение
      if (!existingAssignment)
        throw new EntityNotFoundException(new ExpressionToStringConverter<Assignment>().Convert(assignmentFilter));
    }

    // Получаем бизнес правило для обновления
    var businessRule = await _unitOfWork.BusinessRules
                                        .FirstAsync(filterByID, cancellationToken);

    // Преобразование DTO в доменную модель назначения
    _mapper.Map(request.UpdateBusinessRule, businessRule);

    // Обновление бизнес-правила в репозитории
    _unitOfWork.BusinessRules
               .ReplaceOne(businessRule);

    await _unitOfWork.CompleteAsync(cancellationToken);

    // Возврат успешного завершения операции
    return Unit.Value;
  }
}

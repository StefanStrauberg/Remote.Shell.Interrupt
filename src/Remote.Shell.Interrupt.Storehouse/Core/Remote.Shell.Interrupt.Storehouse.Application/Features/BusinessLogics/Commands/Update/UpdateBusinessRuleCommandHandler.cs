namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics.Commands.Update;

public record UpdateBusinessRuleCommand(UpdateBusinessRuleDTO UpdateBusinessRule) : ICommand;

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
    // Проверка существует ли бизнес-правило с указанным ID
    var existingUpdatingBusinessRule = await _unitOfWork.BusinessRules
                                                        .AnyByIdAsync(request.UpdateBusinessRule.Id,
                                                                      cancellationToken);

    // Если бизнес-правило не найдено, выбрасываем исключение
    if (!existingUpdatingBusinessRule)
      throw new EntityNotFoundById(typeof(BusinessRule),
                                   request.UpdateBusinessRule.Id.ToString());

    // Проверка существует ли назначение с указанным ID
    var existingAssignment = await _unitOfWork.Assignments
                                              .AnyByIdAsync(request.UpdateBusinessRule.AssignmentId,
                                                            cancellationToken);

    // Если назначение не найдено, выбрасываем исключение
    if (!existingAssignment)
      throw new EntityNotFoundById(typeof(Assignment),
                                   request.UpdateBusinessRule.AssignmentId.ToString()!);

    // Проверка существует ли родительское бизнес-правило
    if (request.UpdateBusinessRule.ParentId is not null)
    {
      // Проверка существует ли родительское бизнес-правило с указанным ID
      var existingParentOfUpdatingBusinessRule = await _unitOfWork.BusinessRules
                                                                  .AnyByIdAsync(request.UpdateBusinessRule.ParentId.Value,
                                                                                cancellationToken);

      // Если родительское бизнес-правило не найдено, выбрасываем исключение
      if (!existingParentOfUpdatingBusinessRule)
        throw new EntityNotFoundById(typeof(BusinessRule),
                                     request.UpdateBusinessRule.Id.ToString());
    }

    // Получаем бизнес правило для обновления
    var businessRule = await _unitOfWork.BusinessRules
                                        .FirstByIdAsync(request.UpdateBusinessRule.Id,
                                                        cancellationToken);

    // Преобразование DTO в доменную модель назначения
    _mapper.Map(request.UpdateBusinessRule, businessRule);

    // Обновление бизнес-правила в репозитории
    _unitOfWork.BusinessRules
               .ReplaceOne(businessRule);

    // Подтверждаем изменения
    _unitOfWork.Complete();

    // Возврат успешного завершения операции
    return Unit.Value;
  }
}

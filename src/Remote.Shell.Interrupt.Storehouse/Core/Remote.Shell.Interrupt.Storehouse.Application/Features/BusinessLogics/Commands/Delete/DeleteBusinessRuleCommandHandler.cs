namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics.Commands.Delete;

public record DeleteBusinessRuleByIdCommand(Guid Id)
  : ICommand;

internal class DeleteBusinessRuleByIdCommandHandler(IUnitOfWork unitOfWork)
  : ICommandHandler<DeleteBusinessRuleByIdCommand, Unit>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));

  async Task<Unit> IRequestHandler<DeleteBusinessRuleByIdCommand, Unit>.Handle(DeleteBusinessRuleByIdCommand request,
                                                                               CancellationToken cancellationToken)
  {
    // Проверка существования бизнес-правила с данным ID
    var existingBusinessRuleById = await _unitOfWork.BusinessRules.AnyByIdAsync(request.Id,
                                                                                cancellationToken);

    // Если бизнес-правило не найдено — исключение
    if (!existingBusinessRuleById)
      throw new EntityNotFoundById(typeof(BusinessRule),
                                   request.Id.ToString());

    // Получаем бизнес-правило для удаления
    var businessRuleToDelete = await _unitOfWork.BusinessRules
                                                .FirstByIdAsync(request.Id,
                                                                cancellationToken);

    // Если у сущности есть родитель
    if (businessRuleToDelete.Parent != null)
    {
      // Проверка существования родителя бизнес-правила
      var existingParntBusinessRuleById = await _unitOfWork.BusinessRules.AnyByIdAsync(request.Id,
                                                                                       cancellationToken);

      // Если родитель бизнес-правило с таким ID не найдено, выбрасываем исключение
      if (!existingBusinessRuleById)
        throw new EntityNotFoundById(typeof(BusinessRule),
                                     request.Id.ToString());

      // Получаем родитель бизнес-правило
      var parentBusinessRule = await _unitOfWork.BusinessRules
                                                .FirstByIdAsync(businessRuleToDelete.ParentId!.Value,
                                                                cancellationToken);

      // Удалить ID текущей сущности из списка дочерних узлов родителя
      parentBusinessRule.Children
                        .Remove(businessRuleToDelete);

      // Если текущая сущность имеет дочерние узлы, добавить их к родителю
      if (businessRuleToDelete.Children.Count != 0)
        parentBusinessRule.Children
                          .AddRange(businessRuleToDelete.Children
                                                        .Select(child => child));

      // Сохранить изменения в родительской сущности
      _unitOfWork.BusinessRules
                 .ReplaceOne(parentBusinessRule);
    }

    // Обновить дочерние узлы текущей сущности, чтобы установить их нового родителя
    foreach (var child in businessRuleToDelete.Children)
    {
      // Проверка существует ли дочернее бизнес-правило
      var existingAssignment = await _unitOfWork.BusinessRules
                                                .AnyByIdAsync(child.Id,
                                                              cancellationToken);

      // Если дочернее бизнес-правило не найдено, выбрасываем исключение
      if (!existingAssignment)
        throw new EntityNotFoundById(typeof(Assignment),
                                     child.Id.ToString());

      // Находим дочернее бизнес-правило
      var childBusinessRule = await _unitOfWork.BusinessRules
                                               .FirstByIdAsync(child.Id,
                                                               cancellationToken);

      // Установить родителя для дочерней сущности
      childBusinessRule.ParentId = businessRuleToDelete.ParentId;

      // Сохранить изменения в дочерней сущности
      _unitOfWork.BusinessRules.ReplaceOne(childBusinessRule);
    }

    // Удалить текущую сущность из репозитория
    _unitOfWork.BusinessRules
               .DeleteOne(businessRuleToDelete);

    await _unitOfWork.CompleteAsync(cancellationToken);

    // Возвратить успешное завершение операции
    return Unit.Value;
  }
}

namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics.Commands.Delete;

public record DeleteBusinessRuleByExpressionCommand(Expression<Func<BusinessRule, bool>> FilterExpression)
  : ICommand;

internal class DeleteBusinessRuleByExpressionCommandHandler(IUnitOfWork unitOfWork)
  : ICommandHandler<DeleteBusinessRuleByExpressionCommand, Unit>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));

  async Task<Unit> IRequestHandler<DeleteBusinessRuleByExpressionCommand, Unit>.Handle(DeleteBusinessRuleByExpressionCommand request,
                                                                                       CancellationToken cancellationToken)
  {
    // Найти сущность, которую нужно удалить, по фильтру из команды
    var businessRuleToDelete = await _unitOfWork.BusinessRules
                                                .FirstAsync(request.FilterExpression, cancellationToken)
      ?? throw new EntityNotFoundException(new ExpressionToStringConverter<BusinessRule>().Convert(request.FilterExpression));

    // Проверить, есть ли у сущности родитель
    if (businessRuleToDelete.Parent != null)
    {
      // Создать фильтр для поиска родительской сущности
      var parentFilter = (Expression<Func<BusinessRule, bool>>)(x => x.Id == businessRuleToDelete.ParentId);
      var parentBusinessRule = await _unitOfWork.BusinessRules
                                                .FirstAsync(parentFilter, cancellationToken);

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
      // Создать фильтр для поиска дочерней сущности
      var childFilter = (Expression<Func<BusinessRule, bool>>)(x => x.Id == child.Id);
      var childBusinessRule = await _unitOfWork.BusinessRules
                                               .FirstAsync(childFilter, cancellationToken);

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

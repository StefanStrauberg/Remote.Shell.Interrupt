namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics.Commands.Delete;

public record DeleteBusinessRuleByExpressionCommand(Expression<Func<BusinessRule, bool>> FilterExpression)
  : ICommand;

internal class DeleteBusinessRuleByExpressionCommandHandler(IBusinessRuleRepository businessRuleRepository)
  : ICommandHandler<DeleteBusinessRuleByExpressionCommand, Unit>
{
  readonly IBusinessRuleRepository _businessRuleRepository = businessRuleRepository
    ?? throw new ArgumentNullException(nameof(businessRuleRepository));

  async Task<Unit> IRequestHandler<DeleteBusinessRuleByExpressionCommand, Unit>.Handle(DeleteBusinessRuleByExpressionCommand request,
                                                                                       CancellationToken cancellationToken)
  {
    // Найти сущность, которую нужно удалить, по фильтру из команды
    var businessRuleToDelete = await _businessRuleRepository.FindOneAsync(request.FilterExpression,
                                                                          cancellationToken)
      ?? throw new EntityNotFoundException(new ExpressionToStringConverter<BusinessRule>().Convert(request.FilterExpression));

    // Проверить, есть ли у сущности родитель
    if (businessRuleToDelete.Parent != null)
    {
      // Создать фильтр для поиска родительской сущности
      var parentFilter = (Expression<Func<BusinessRule, bool>>)(x => x.Id == businessRuleToDelete.ParentId);
      var parentBusinessRule = await _businessRuleRepository.FindOneAsync(parentFilter, cancellationToken);

      // Если родительская сущность найдена
      if (parentBusinessRule != null)
      {
        // Удалить ID текущей сущности из списка дочерних узлов родителя
        parentBusinessRule.Children.Remove(businessRuleToDelete);

        // Если текущая сущность имеет дочерние узлы, добавить их к родителю
        if (businessRuleToDelete.Children.Count != 0)
          parentBusinessRule.Children.AddRange(businessRuleToDelete.Children.Select(child => child));

        // Сохранить изменения в родительской сущности
        await _businessRuleRepository.ReplaceOneAsync(parentFilter, parentBusinessRule, cancellationToken);
      }
    }

    // Обновить дочерние узлы текущей сущности, чтобы установить их нового родителя
    foreach (var child in businessRuleToDelete.Children)
    {
      // Создать фильтр для поиска дочерней сущности
      var childFilter = (Expression<Func<BusinessRule, bool>>)(x => x.Id == child.Id);
      var childBusinessRule = await _businessRuleRepository.FindOneAsync(childFilter, cancellationToken);

      // Если дочерняя сущность найдена
      if (childBusinessRule != null)
      {
        // Установить родителя для дочерней сущности
        childBusinessRule.ParentId = businessRuleToDelete.ParentId;
        // Сохранить изменения в дочерней сущности
        await _businessRuleRepository.ReplaceOneAsync(childFilter, childBusinessRule, cancellationToken);
      }
    }

    // Удалить текущую сущность из репозитория
    await _businessRuleRepository.DeleteOneAsync(filterExpression: request.FilterExpression,
                                                 cancellationToken: cancellationToken);

    // Возвратить успешное завершение операции
    return Unit.Value;
  }
}

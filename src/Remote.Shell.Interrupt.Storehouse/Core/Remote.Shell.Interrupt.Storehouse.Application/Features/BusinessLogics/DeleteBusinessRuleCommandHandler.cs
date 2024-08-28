namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics;

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
    var businessRuleToDelete = await _businessRuleRepository.FindOneAsync(request.FilterExpression, cancellationToken)
      ?? throw new EntityNotFoundException(new ExpressionToStringConverter<BusinessRule>().Convert(request.FilterExpression));

    // Если узел имеет дочерние узлы
    if (businessRuleToDelete.Children.Any())
    {
      // Если у узла есть родитель
      if (businessRuleToDelete.ParentId != null)
      {
        // Найти родительский узел
        var parentFilter = (Expression<Func<BusinessRule, bool>>)(x => x.Id == businessRuleToDelete.ParentId);
        var parentBusinessRule = await _businessRuleRepository.FindOneAsync(parentFilter, cancellationToken);

        if (parentBusinessRule != null)
        {
          // Удалить ID текущего узла из дочерних узлов родителя
          parentBusinessRule.Children.Remove(businessRuleToDelete.Id);

          // Добавить ID дочерних узлов к родителю
          parentBusinessRule.Children.AddRange(businessRuleToDelete.Children.Select(child => child));

          // Обновить родительский узел
          await _businessRuleRepository.ReplaceOneAsync(parentFilter, parentBusinessRule, cancellationToken);
        }
      }

      // Обновить дочерние узлы, чтобы установить их нового родителя
      foreach (var childId in businessRuleToDelete.Children.Select(child => child))
      {
        var childFilter = (Expression<Func<BusinessRule, bool>>)(x => x.Id == childId);
        var childBusinessRule = await _businessRuleRepository.FindOneAsync(childFilter, cancellationToken);

        if (childBusinessRule != null)
        {
          childBusinessRule.ParentId = businessRuleToDelete.ParentId;
          await _businessRuleRepository.ReplaceOneAsync(childFilter, childBusinessRule, cancellationToken);
        }
      }
    }

    await _businessRuleRepository.DeleteOneAsync(filterExpression: request.FilterExpression,
                                                 cancellationToken: cancellationToken);
    return Unit.Value;
  }
}

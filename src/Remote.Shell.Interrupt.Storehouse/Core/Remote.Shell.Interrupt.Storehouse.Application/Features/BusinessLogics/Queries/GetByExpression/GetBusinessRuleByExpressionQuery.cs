namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics.Queries.GetByExpression;

public record GetBusinessRuleByExpressionQuery(Expression<Func<BusinessRule, bool>> FilterExpression)
  : IQuery<BusinessRuleDetailDTO>;

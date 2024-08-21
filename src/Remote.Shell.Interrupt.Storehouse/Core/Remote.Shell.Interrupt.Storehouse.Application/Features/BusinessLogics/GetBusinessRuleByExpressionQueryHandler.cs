namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics;

public record GetBusinessRuleByExpressionQuery(Expression<Func<BusinessRule, bool>> FilterExpression)
  : IQuery<BusinessRule>;

internal class GetBusinessRuleByExpressionQueryHandler(IBusinessRuleRepository businessRuleRepository)
  : IQueryHandler<GetBusinessRuleByExpressionQuery, BusinessRule>
{
  readonly IBusinessRuleRepository _businessRuleRepository = businessRuleRepository
    ?? throw new ArgumentNullException(nameof(businessRuleRepository));

  async Task<BusinessRule> IRequestHandler<GetBusinessRuleByExpressionQuery, BusinessRule>.Handle(GetBusinessRuleByExpressionQuery request,
                                                                                                  CancellationToken cancellationToken)
  {
    var businessRule = await _businessRuleRepository.FindOneAsync(filterExpression: request.FilterExpression,
                                                                  cancellationToken: cancellationToken)
      ?? throw new EntityNotFoundException(request.FilterExpression.Name!);

    return businessRule;
  }
}

namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics;

public record GetBusinessRuleByExpressionQuery(Expression<Func<BusinessRule, bool>> FilterExpression)
  : IQuery<BusinessRuleDTO>;

internal class GetBusinessRuleByExpressionQueryHandler(IBusinessRuleRepository businessRuleRepository)
  : IQueryHandler<GetBusinessRuleByExpressionQuery, BusinessRuleDTO>
{
  readonly IBusinessRuleRepository _businessRuleRepository = businessRuleRepository
    ?? throw new ArgumentNullException(nameof(businessRuleRepository));

  async Task<BusinessRuleDTO> IRequestHandler<GetBusinessRuleByExpressionQuery, BusinessRuleDTO>.Handle(GetBusinessRuleByExpressionQuery request,
                                                                                                        CancellationToken cancellationToken)
  {
    var businessRule = await _businessRuleRepository.FindOneAsync(filterExpression: request.FilterExpression,
                                                                  cancellationToken: cancellationToken)
      ?? throw new EntityNotFoundException(request.FilterExpression.Name!);
    var businessRuleDTO = businessRule.Adapt<BusinessRuleDTO>();

    return businessRuleDTO;
  }
}

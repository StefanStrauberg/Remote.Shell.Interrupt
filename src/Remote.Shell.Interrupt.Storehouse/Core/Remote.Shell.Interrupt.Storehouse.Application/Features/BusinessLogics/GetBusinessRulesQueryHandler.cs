namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics;

public record GetBusinessRulesQuery() : IQuery<IEnumerable<BusinessRule>>;

internal class GetBusinessRulesQueryHandler(IBusinessRuleRepository businessRuleRepository)
  : IQueryHandler<GetBusinessRulesQuery, IEnumerable<BusinessRule>>
{
  readonly IBusinessRuleRepository _businessRuleRepository = businessRuleRepository
    ?? throw new ArgumentNullException(nameof(businessRuleRepository));

  async Task<IEnumerable<BusinessRule>> IRequestHandler<GetBusinessRulesQuery, IEnumerable<BusinessRule>>.Handle(GetBusinessRulesQuery request,
                                                                                                                 CancellationToken cancellationToken)
  {
    var businessRules = await _businessRuleRepository.GetAllAsync(cancellationToken);

    return businessRules;
  }
}

namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics;

public record GetBusinessRulesQuery() : IQuery<IEnumerable<BusinessRuleDTO>>;

internal class GetBusinessRulesQueryHandler(IBusinessRuleRepository businessRuleRepository)
  : IQueryHandler<GetBusinessRulesQuery, IEnumerable<BusinessRuleDTO>>
{
  readonly IBusinessRuleRepository _businessRuleRepository = businessRuleRepository
    ?? throw new ArgumentNullException(nameof(businessRuleRepository));

  async Task<IEnumerable<BusinessRuleDTO>> IRequestHandler<GetBusinessRulesQuery, IEnumerable<BusinessRuleDTO>>.Handle(GetBusinessRulesQuery request,
                                                                                                                       CancellationToken cancellationToken)
  {
    var businessRules = await _businessRuleRepository.GetAllAsync(cancellationToken);
    var businessRulesDTOs = businessRules.Adapt<IEnumerable<BusinessRuleDTO>>();

    return businessRulesDTOs;
  }
}

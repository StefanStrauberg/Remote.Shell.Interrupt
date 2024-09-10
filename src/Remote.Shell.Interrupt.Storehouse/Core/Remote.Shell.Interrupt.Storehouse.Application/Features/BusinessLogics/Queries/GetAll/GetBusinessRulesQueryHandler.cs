namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics.Queries.GetAll;

internal class GetBusinessRulesQueryHandler(IBusinessRuleRepository businessRuleRepository,
                                            IMapper mapper)
  : IQueryHandler<GetBusinessRulesQuery, IEnumerable<BusinessRuleDTO>>
{
  readonly IBusinessRuleRepository _businessRuleRepository = businessRuleRepository
    ?? throw new ArgumentNullException(nameof(businessRuleRepository));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<IEnumerable<BusinessRuleDTO>> IRequestHandler<GetBusinessRulesQuery, IEnumerable<BusinessRuleDTO>>.Handle(GetBusinessRulesQuery request,
                                                                                                                       CancellationToken cancellationToken)
  {
    var businessRules = await _businessRuleRepository.GetAllWithChildrenAsync(cancellationToken);
    var businessRulesDTOs = _mapper.Map<IEnumerable<BusinessRuleDTO>>(businessRules);

    return businessRulesDTOs;
  }
}

namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics.Queries.GetAll;

public record GetBusinessRulesQuery() : IQuery<IEnumerable<BusinessRuleDTO>>;

internal class GetBusinessRulesQueryHandler(IUnitOfWork unitOfWork,
                                            IMapper mapper)
  : IQueryHandler<GetBusinessRulesQuery, IEnumerable<BusinessRuleDTO>>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<IEnumerable<BusinessRuleDTO>> IRequestHandler<GetBusinessRulesQuery, IEnumerable<BusinessRuleDTO>>.Handle(GetBusinessRulesQuery request,
                                                                                                                       CancellationToken cancellationToken)
  {
    var businessRules = await _unitOfWork.BusinessRules
                                         .GetAllWithChildrenAsync(cancellationToken);

    var businessRulesDTOs = _mapper.Map<IEnumerable<BusinessRuleDTO>>(businessRules);

    return businessRulesDTOs;
  }
}

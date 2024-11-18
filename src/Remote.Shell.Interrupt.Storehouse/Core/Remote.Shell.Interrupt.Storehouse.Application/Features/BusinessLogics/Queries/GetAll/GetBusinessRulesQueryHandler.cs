namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics.Queries.GetAll;

public record GetBusinessRulesTreeQuery() : IQuery<BusinessRuleDTO>;

internal class GetBusinessRulesQueryHandler(IUnitOfWork unitOfWork,
                                            IMapper mapper)
  : IQueryHandler<GetBusinessRulesTreeQuery, BusinessRuleDTO>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<BusinessRuleDTO> IRequestHandler<GetBusinessRulesTreeQuery, BusinessRuleDTO>.Handle(GetBusinessRulesTreeQuery request,
                                                                                                 CancellationToken cancellationToken)
  {
    var businessRules = await _unitOfWork.BusinessRules
                                         .GetBusinessRulesTreeAsync(cancellationToken);

    var businessRulesDTOs = _mapper.Map<BusinessRuleDTO>(businessRules);

    return businessRulesDTOs;
  }
}

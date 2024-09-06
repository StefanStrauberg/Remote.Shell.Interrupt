namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics.Queries.GetByExpression;

internal class GetBusinessRuleByExpressionQueryHandler(IBusinessRuleRepository businessRuleRepository,
                                                       IMapper mapper)
  : IQueryHandler<GetBusinessRuleByExpressionQuery, BusinessRuleDetailDTO>
{
  readonly IBusinessRuleRepository _businessRuleRepository = businessRuleRepository
    ?? throw new ArgumentNullException(nameof(businessRuleRepository));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));


  async Task<BusinessRuleDetailDTO> IRequestHandler<GetBusinessRuleByExpressionQuery, BusinessRuleDetailDTO>.Handle(GetBusinessRuleByExpressionQuery request,
                                                                                                                    CancellationToken cancellationToken)
  {
    var businessRule = await _businessRuleRepository.FindOneAsync(filterExpression: request.FilterExpression,
                                                                  cancellationToken: cancellationToken)
      ?? throw new EntityNotFoundException(request.FilterExpression.Name!);
    var businessRuleDTO = _mapper.Map<BusinessRuleDetailDTO>(businessRule);

    return businessRuleDTO;
  }
}

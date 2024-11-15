namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics.Queries.GetByExpression;

public record GetBusinessRuleByIdQuery(Guid Id)
  : IQuery<BusinessRuleDetailDTO>;

internal class GetBusinessRuleByIdQueryHandler(IUnitOfWork unitOfWork,
                                               IMapper mapper)
  : IQueryHandler<GetBusinessRuleByIdQuery, BusinessRuleDetailDTO>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));


  async Task<BusinessRuleDetailDTO> IRequestHandler<GetBusinessRuleByIdQuery, BusinessRuleDetailDTO>.Handle(GetBusinessRuleByIdQuery request,
                                                                                                            CancellationToken cancellationToken)
  {
    // Проверка существует ли бизнес правило
    var existingAssignment = await _unitOfWork.BusinessRules
                                              .AnyByIdAsync(request.Id,
                                                            cancellationToken);

    // Если бизнес правило не найдено, выбрасываем исключение
    if (!existingAssignment)
      throw new EntityNotFoundById(typeof(BusinessRule),
                                   request.Id.ToString());

    // Находим бизнес правило
    var businessRule = await _unitOfWork.BusinessRules.FirstByIdAsync(request.Id,
                                                                      cancellationToken);

    var businessRuleDTO = _mapper.Map<BusinessRuleDetailDTO>(businessRule);

    return businessRuleDTO;
  }
}

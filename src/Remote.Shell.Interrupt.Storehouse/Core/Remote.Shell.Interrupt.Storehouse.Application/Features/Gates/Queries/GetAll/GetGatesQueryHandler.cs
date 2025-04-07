namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Queries.GetAll;

public record GetGatesQuery(RequestParameters RequestParameters) 
  : IQuery<PagedList<GateDTO>>;

internal class GetGatesQueryHandler(IUnitOfWork unitOfWork,
                                    IMapper mapper)
  : IQueryHandler<GetGatesQuery, PagedList<GateDTO>>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<PagedList<GateDTO>> IRequestHandler<GetGatesQuery, PagedList<GateDTO>>.Handle(GetGatesQuery request,
                                                                                           CancellationToken cancellationToken)
  {
    var gates = await _unitOfWork.GateRepository
                                 .GetGatesByQueryAsync(request.RequestParameters,
                                                   cancellationToken);
    var count = await _unitOfWork.GateRepository
                                 .GetCountAsync(request.RequestParameters,
                                                cancellationToken);
    var result = _mapper.Map<List<GateDTO>>(gates);

    return new PagedList<GateDTO>(result,
                                  count,
                                  request.RequestParameters
                                         .PageNumber,
                                  request.RequestParameters
                                         .PageSize);
  }
}

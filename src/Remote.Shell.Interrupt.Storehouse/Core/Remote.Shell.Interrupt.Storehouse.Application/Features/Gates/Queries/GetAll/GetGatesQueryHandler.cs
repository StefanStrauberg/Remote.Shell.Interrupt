namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Queries.GetAll;

public record GetGatesQuery(RequestParameters RequestParameters) 
  : IQuery<PagedList<GateDTO>>;

internal class GetGatesQueryHandler(IGateUnitOfWork gateUnitOfWork,
                                    IMapper mapper)
  : IQueryHandler<GetGatesQuery, PagedList<GateDTO>>
{
  async Task<PagedList<GateDTO>> IRequestHandler<GetGatesQuery, PagedList<GateDTO>>.Handle(GetGatesQuery request,
                                                                                           CancellationToken cancellationToken)
  {
    var gates = await gateUnitOfWork.GateRepository
                                    .GetManyShortAsync(request.RequestParameters,
                                                       cancellationToken);
                                                   
    if (!gates.Any())
      return new PagedList<GateDTO>([],0,0,0);

    var count = await gateUnitOfWork.GateRepository
                                    .GetCountAsync(request.RequestParameters,
                                                   cancellationToken);

    var result = mapper.Map<IEnumerable<GateDTO>>(gates);

    return new PagedList<GateDTO>(result,
                                  count,
                                  request.RequestParameters
                                         .PageNumber,
                                  request.RequestParameters
                                         .PageSize);
  }
}

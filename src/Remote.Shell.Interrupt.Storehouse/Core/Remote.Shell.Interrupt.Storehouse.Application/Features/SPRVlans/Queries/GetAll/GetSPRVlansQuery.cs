namespace Remote.Shell.Interrupt.Storehouse.Application.Features.SPRVlans.Queries.GetAll;

public record GetSPRVlansQuery(RequestParameters RequestParameters) 
  : IQuery<PagedList<SPRVlanDTO>>;

internal class GetSPRVlansQueryHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                       IMapper mapper) 
  : IQueryHandler<GetSPRVlansQuery, PagedList<SPRVlanDTO>>
{
  async Task<PagedList<SPRVlanDTO>> IRequestHandler<GetSPRVlansQuery, PagedList<SPRVlanDTO>>.Handle(GetSPRVlansQuery request,
                                                                                                    CancellationToken cancellationToken)
  {
    var sprVlans = await locBillUnitOfWork.SPRVlans
                                          .GetManyShortAsync(request.RequestParameters,
                                                             cancellationToken);

    if (!sprVlans.Any())
      return new PagedList<SPRVlanDTO>([],0,0,0);

    var count = await locBillUnitOfWork.SPRVlans
                                       .GetCountAsync(request.RequestParameters,
                                                      cancellationToken);
                                                
    var result = mapper.Map<List<SPRVlanDTO>>(sprVlans);

    return new PagedList<SPRVlanDTO>(result,
                                     count,
                                     request.RequestParameters
                                             .PageNumber,
                                     request.RequestParameters
                                             .PageSize);
  }
}


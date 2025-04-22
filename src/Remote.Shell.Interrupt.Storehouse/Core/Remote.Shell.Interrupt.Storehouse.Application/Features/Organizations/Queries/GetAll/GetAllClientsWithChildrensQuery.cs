namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetAll;

public record GetAllClientsWithChildrensQuery(RequestParameters RequestParameters) 
  : IQuery<PagedList<DetailClientDTO>>;

internal class GetAllClientsWithChildrensQueryHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                                      IMapper mapper)
  : IQueryHandler<GetAllClientsWithChildrensQuery, PagedList<DetailClientDTO>>
{
  async Task<PagedList<DetailClientDTO>> IRequestHandler<GetAllClientsWithChildrensQuery, PagedList<DetailClientDTO>>.Handle(GetAllClientsWithChildrensQuery request,
                                                                                                                             CancellationToken cancellationToken)
  {
    var clients = await locBillUnitOfWork.Clients
                                         .GetManyWithChildrenAsync(request.RequestParameters,
                                                                   cancellationToken);
                                                                        
    if (!clients.Any())
      return new PagedList<DetailClientDTO>([],0,0,0);

    var count = await locBillUnitOfWork.Clients
                                       .GetCountAsync(request.RequestParameters,
                                                      cancellationToken);

    var result = mapper.Map<IEnumerable<DetailClientDTO>>(clients);

    return new PagedList<DetailClientDTO>(result,
                                          count,
                                          request.RequestParameters.PageNumber,
                                          request.RequestParameters.PageSize);
  }
}
namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetAll;

public record GetAllShortClientsQuery(RequestParameters RequestParameters) 
  : IQuery<PagedList<ShortClientDTO>>;

internal class GetAllShortClientsQueryHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                              IMapper mapper)
  : IQueryHandler<GetAllShortClientsQuery, PagedList<ShortClientDTO>>
{
  async Task<PagedList<ShortClientDTO>> IRequestHandler<GetAllShortClientsQuery, PagedList<ShortClientDTO>>.Handle(GetAllShortClientsQuery request,
                                                                                                         CancellationToken cancellationToken)
  {
    var clients = await locBillUnitOfWork.Clients
                                         .GetManyShortAsync(request.RequestParameters,
                                                            cancellationToken);

    if (!clients.Any())
      return new PagedList<ShortClientDTO>([],0,0,0);

    var count = await locBillUnitOfWork.Clients
                                       .GetCountAsync(request.RequestParameters,
                                                      cancellationToken);

    var result = mapper.Map<IEnumerable<ShortClientDTO>>(clients);

    return new PagedList<ShortClientDTO>(result,
                                         count,
                                         request.RequestParameters.PageNumber,
                                         request.RequestParameters.PageSize);
  }
}

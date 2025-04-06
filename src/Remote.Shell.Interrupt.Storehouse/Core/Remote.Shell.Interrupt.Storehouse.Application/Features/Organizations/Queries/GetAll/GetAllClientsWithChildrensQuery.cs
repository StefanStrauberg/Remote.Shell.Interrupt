namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetAll;

public record GetAllClientsWithChildrensQuery(RequestParameters RequestParameters) 
  : IQuery<PagedList<DetailClientDTO>>;

internal class GetAllClientsWithChildrensQueryHandler(IUnitOfWork unitOfWork,
                                                      IMapper mapper)
  : IQueryHandler<GetAllClientsWithChildrensQuery, PagedList<DetailClientDTO>>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<PagedList<DetailClientDTO>> IRequestHandler<GetAllClientsWithChildrensQuery, PagedList<DetailClientDTO>>.Handle(GetAllClientsWithChildrensQuery request,
                                                                                                                             CancellationToken cancellationToken)
  {
    var clients = await _unitOfWork.Clients
                                   .GetAllClientsWithChildrensAsync(request.RequestParameters,
                                                                    cancellationToken);
    var count = await _unitOfWork.Clients
                                 .GetCountAsync(request.RequestParameters,
                                                cancellationToken);

    var result = _mapper.Map<List<DetailClientDTO>>(clients);

    return new PagedList<DetailClientDTO>(result,
                                          count,
                                          request.RequestParameters.PageNumber,
                                          request.RequestParameters.PageSize);
  }
}
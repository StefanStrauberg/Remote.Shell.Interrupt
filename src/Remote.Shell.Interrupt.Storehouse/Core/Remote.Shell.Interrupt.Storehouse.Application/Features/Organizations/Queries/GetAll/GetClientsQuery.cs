namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetAll;

public record GetAllShortClientsQuery(RequestParameters RequestParameters) 
  : IQuery<PagedList<ClientDTO>>;

internal class GetAllShortClientsQueryHandler(IUnitOfWork unitOfWork,
                                              IMapper mapper)
  : IQueryHandler<GetAllShortClientsQuery, PagedList<ClientDTO>>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<PagedList<ClientDTO>> IRequestHandler<GetAllShortClientsQuery, PagedList<ClientDTO>>.Handle(GetAllShortClientsQuery request,
                                                                                                         CancellationToken cancellationToken)
  {
    var clients = await _unitOfWork.Clients
                                   .GetAllShortClientsAsync(request.RequestParameters,
                                                            cancellationToken);
    var count = await _unitOfWork.Clients
                                 .GetCountAsync(request.RequestParameters,
                                                cancellationToken);

    var result = _mapper.Map<List<ClientDTO>>(clients);

    return new PagedList<ClientDTO>(result,
                                       count,
                                       request.RequestParameters.PageNumber,
                                       request.RequestParameters.PageSize);
  }
}

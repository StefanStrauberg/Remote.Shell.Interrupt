namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetAll;

public record GetClientsQuery(RequestParameters RequestParameters) : IQuery<PagedList<ClientDTO>>;

internal class GetClientsQueryHandler(IUnitOfWork unitOfWork,
                                         IMapper mapper)
  : IQueryHandler<GetClientsQuery, PagedList<ClientDTO>>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<PagedList<ClientDTO>> IRequestHandler<GetClientsQuery, PagedList<ClientDTO>>.Handle(GetClientsQuery request,
                                                                                                 CancellationToken cancellationToken)
  {
    var clients = await _unitOfWork.Clients
                                   .GetAllShortAsync(request.RequestParameters,
                                                     cancellationToken);
    var count = await _unitOfWork.Clients
                                 .GetCountAsync(cancellationToken);

    var result = _mapper.Map<List<ClientDTO>>(clients);

    return new PagedList<ClientDTO>(result,
                                       count,
                                       request.RequestParameters.PageNumber,
                                       request.RequestParameters.PageSize);
  }
}

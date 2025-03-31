namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetAll;

public record GetClientsCODQuery(RequestParameters RequestParameters) 
  : IQuery<PagedList<ClientCODDTO>>;

internal class GetClientsCODQueryHandler(IUnitOfWork unitOfWork,
                                         IMapper mapper)
  : IQueryHandler<GetClientsCODQuery, PagedList<ClientCODDTO>>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<PagedList<ClientCODDTO>> IRequestHandler<GetClientsCODQuery, PagedList<ClientCODDTO>>.Handle(GetClientsCODQuery request,
                                                                                                          CancellationToken cancellationToken)
  {
    var clients = await _unitOfWork.ClientCODLs
                                   .GetAllShortAsync(request.RequestParameters,
                                                     cancellationToken);
    var count = await _unitOfWork.ClientCODLs
                                 .GetCountAsync(cancellationToken);

    var result = _mapper.Map<List<ClientCODDTO>>(clients);

    return new PagedList<ClientCODDTO>(result,
                                       count,
                                       request.RequestParameters.PageNumber,
                                       request.RequestParameters.PageSize);
  }
}

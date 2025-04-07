namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetAll;

public record GetAllShortClientsQuery(RequestParameters RequestParameters) 
  : IQuery<PagedList<ShortClientDTO>>;

internal class GetAllShortClientsQueryHandler(IUnitOfWork unitOfWork,
                                              IMapper mapper)
  : IQueryHandler<GetAllShortClientsQuery, PagedList<ShortClientDTO>>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<PagedList<ShortClientDTO>> IRequestHandler<GetAllShortClientsQuery, PagedList<ShortClientDTO>>.Handle(GetAllShortClientsQuery request,
                                                                                                         CancellationToken cancellationToken)
  {
    var clients = await _unitOfWork.Clients
                                   .GetShortClientsByQueryAsync(request.RequestParameters,
                                                                cancellationToken);
    var count = await _unitOfWork.Clients
                                 .GetCountAsync(request.RequestParameters,
                                                cancellationToken);

    var result = _mapper.Map<List<ShortClientDTO>>(clients);

    return new PagedList<ShortClientDTO>(result,
                                         count,
                                         request.RequestParameters.PageNumber,
                                         request.RequestParameters.PageSize);
  }
}

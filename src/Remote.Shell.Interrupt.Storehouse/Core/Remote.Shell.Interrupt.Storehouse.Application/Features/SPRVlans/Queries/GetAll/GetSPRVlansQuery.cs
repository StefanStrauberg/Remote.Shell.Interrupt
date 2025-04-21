
namespace Remote.Shell.Interrupt.Storehouse.Application.Features.SPRVlans.Queries.GetAll;

public record GetSPRVlansQuery(RequestParameters RequestParameters) 
  : IQuery<PagedList<SPRVlanDTO>>;

internal class GetSPRVlansQueryHandler(IUnitOfWork unitOfWork,
                                       IMapper mapper) 
  : IQueryHandler<GetSPRVlansQuery, PagedList<SPRVlanDTO>>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
        ?? throw new ArgumentNullException(nameof(unitOfWork));
    readonly IMapper _mapper = mapper
        ?? throw new ArgumentNullException(nameof(mapper));

  async Task<PagedList<SPRVlanDTO>> IRequestHandler<GetSPRVlansQuery, PagedList<SPRVlanDTO>>.Handle(GetSPRVlansQuery request,
                                                                                                    CancellationToken cancellationToken)
  {
    var sprVlans = await _unitOfWork.SPRVlans
                                    .GetManyByQueryAsync(request.RequestParameters,
                                                             cancellationToken);

    var count = await _unitOfWork.SPRVlans
                                 .GetCountAsync(request.RequestParameters,
                                                cancellationToken);
                                                
    var result = _mapper.Map<List<SPRVlanDTO>>(sprVlans);

    return new PagedList<SPRVlanDTO>(result,
                                     count,
                                     request.RequestParameters
                                             .PageNumber,
                                     request.RequestParameters
                                             .PageSize);
  }
}


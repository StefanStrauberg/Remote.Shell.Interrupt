namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetByVlanTag;

public record GetClientsByVlanTagQuery(int VlanTag) : IQuery<IEnumerable<DetailClientDTO>>;

internal class GetClientsByVlanTagQueryHandler(IUnitOfWork unitOfWork,
                                               IMapper mapper)
  : IQueryHandler<GetClientsByVlanTagQuery, IEnumerable<DetailClientDTO>>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<IEnumerable<DetailClientDTO>> IRequestHandler<GetClientsByVlanTagQuery, IEnumerable<DetailClientDTO>>.Handle(GetClientsByVlanTagQuery request,
                                                                                                                          CancellationToken cancellationToken)
  {
    if (request.VlanTag == 0)
      throw new ArgumentException("Invalid VLAN Tag.", nameof(request.VlanTag));

    var getSPRVlansQuery = new GetSPRVlansQuery(new RequestParameters()
                                                {
                                                  Filters = $"IdVlan=={request.VlanTag}"
                                                });

    var getSPRVlansQueryHandler = new GetSPRVlansQueryHandler(unitOfWork, mapper);

    var sprVlans = await ((IRequestHandler<GetSPRVlansQuery, PagedList<SPRVlanDTO>>)getSPRVlansQueryHandler).Handle(getSPRVlansQuery,
                                                                                                                    cancellationToken);

    HashSet<Client> clients = [];

    foreach (var item in sprVlans)
    {
      var client = await _unitOfWork.Clients
                                    .GetOneWithChildrensByQueryAsync(new RequestParameters()
                                                                        {
                                                                          Filters = $"IdClient=={item.IdClient}"
                                                                        }, 
                                                                        cancellationToken);
      clients.Add(client);
    }

    var result = _mapper.Map<IEnumerable<DetailClientDTO>>(clients);

    return result;
  }
}
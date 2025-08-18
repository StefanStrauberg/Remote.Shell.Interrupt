namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetClientsByVlanTag;

public record GetClientsByVlanTagQuery(int VlanTag) 
  : FindEntitiesByFilterQuery<DetailClientDTO>(RequestParametersFactory.ForVlanTag(VlanTag));

internal class GetClientsByVlanTagQueryHandler(ILocBillUnitOfWork unitOfWork,
                                               IClientSpecification clientSpec,
                                               IQueryFilterParser filterParser,
                                               IMapper mapper,
                                               IQueryHandler<GetSPRVlansByFilterQuery, PagedList<SPRVlanDTO>> vlanHandler)
  : FindEntitiesByFilterQueryHandler<Client, DetailClientDTO, GetClientsByVlanTagQuery>(clientSpec, filterParser, mapper)
{
  readonly IQueryFilterParser _filterParser = filterParser;
  readonly IMapper _mapper = mapper;

  public override async Task<PagedList<DetailClientDTO>> Handle(GetClientsByVlanTagQuery request, CancellationToken cancellationToken)
  {
    ValidateRequest(request);

    var vlans = await FetchVlans(request, cancellationToken);
    var clients = await FetchClients(vlans, cancellationToken);

    return MapToPagedList(clients);
  }

  static void ValidateRequest(GetClientsByVlanTagQuery request)
  {
    if (request.VlanTag <= 0)
      throw new ArgumentException("Invalid VLAN Tag.", nameof(request.VlanTag));
  }

  async Task<IEnumerable<SPRVlanDTO>> FetchVlans(GetClientsByVlanTagQuery request, 
                                                 CancellationToken cancellationToken)
  {
    var query = new GetSPRVlansByFilterQuery(RequestParametersFactory.ForVlanTag(request.VlanTag));
    var result = await vlanHandler.Handle(query, cancellationToken);
    
    return result;
  }

  async Task<IEnumerable<Client>> FetchClients(IEnumerable<SPRVlanDTO> vlans,
                                               CancellationToken cancellationToken)
  {
    var clients = new HashSet<Client>();
    
    foreach (var vlan in vlans)
    {
      var parameters = RequestParametersFactory.ForClientId(vlan.IdClient);
      var filter = _filterParser.ParseFilters<Client>(parameters.Filters);
      var spec = BuildSpecification(clientSpec, filter);
      var client = await unitOfWork.Clients.GetOneWithChildrenAsync(spec, cancellationToken);

      if (client is not null)
        clients.Add(client);
    }

    return clients;
  }

  PagedList<DetailClientDTO> MapToPagedList(IEnumerable<Client> clients)
  {
    var dtos = _mapper.Map<IEnumerable<DetailClientDTO>>(clients);

    return PagedList<DetailClientDTO>.Create(dtos, dtos.Count(), new PaginationContext(0, 0));
  }

  public static ISpecification<Client> BuildSpecification(IClientSpecification baseSpec,
                                                          Expression<Func<Client, bool>>? filterExpr)
  {
    var spec = baseSpec.AddInclude(c => c.COD)
                       .AddInclude(c => c.TfPlan!)
                       .AddInclude(c => c.SPRVlans);

    if (filterExpr is not null)
      spec.AddFilter(filterExpr);

    return spec;
  }
  protected override Task<int> CountResultsAsync(ISpecification<Client> specification,
                                                 CancellationToken cancellationToken)
    => throw new NotImplementedException();

  protected override Task<IEnumerable<Client>> FetchEntitiesAsync(ISpecification<Client> specification,
                                                                  CancellationToken cancellationToken)
    => throw new NotImplementedException();
}

namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetClientsByVlanTag;

public record GetClientsByVlanTagQuery(int VlanTag) : IQuery<IEnumerable<DetailClientDTO>>;

internal class GetClientsByVlanTagQueryHandler(ILocBillUnitOfWork unitOfWork,
                                               IClientSpecification clientSpec,
                                               IQueryFilterParser filterParser,
                                               IMapper mapper,
                                               FindEntitiesByFilterQueryHandler<SPRVlan, SPRVlanDTO, GetSPRVlansByFilterQuery> vlanHandler)
  : IQueryHandler<GetClientsByVlanTagQuery, IEnumerable<DetailClientDTO>>
{
  async Task<IEnumerable<DetailClientDTO>> IRequestHandler<GetClientsByVlanTagQuery, IEnumerable<DetailClientDTO>>.Handle(GetClientsByVlanTagQuery request,
                                                                                                                          CancellationToken cancellationToken)
  {
    ValidateRequest(request);

    var vlans = await FetchVlans(request, cancellationToken);
    var clients = await FetchClients(vlans, cancellationToken);

    return MapToDTOList(clients);
  }

  static void ValidateRequest(GetClientsByVlanTagQuery request)
  {
    if (request.VlanTag <= 0)
      throw new ArgumentException("Invalid VLAN Tag.", nameof(request.VlanTag));
  }

  async Task<IEnumerable<SPRVlanDTO>> FetchVlans(GetClientsByVlanTagQuery request, 
                                                 CancellationToken cancellationToken)
  {
    var parameters = RequestParametersFactory.ForVlanTag(request.VlanTag);
    var query = new GetSPRVlansByFilterQuery(parameters);
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
      var filter = filterParser.ParseFilters<Client>(parameters.Filters);
      var spec = BuildSpecification(clientSpec, filter);
      var client = await unitOfWork.Clients.GetOneWithChildrenAsync(spec, cancellationToken);

      if (client is not null)
        clients.Add(client);
    }

    return clients;
  }

  IEnumerable<DetailClientDTO> MapToDTOList(IEnumerable<Client> clients)
    => mapper.Map<IEnumerable<DetailClientDTO>>(clients);

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
}

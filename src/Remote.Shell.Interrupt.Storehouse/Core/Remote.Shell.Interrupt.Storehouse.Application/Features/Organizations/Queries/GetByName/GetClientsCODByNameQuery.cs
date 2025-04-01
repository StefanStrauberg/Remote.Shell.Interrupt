namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetByName;

public record GetClientsByNameQuery(string Name) : IQuery<IEnumerable<DetailClientDTO>>;

internal class GetClientsByNameQueryHandler(IUnitOfWork unitOfWork,
                                            IMapper mapper)
  : IQueryHandler<GetClientsByNameQuery, IEnumerable<DetailClientDTO>>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<IEnumerable<DetailClientDTO>> IRequestHandler<GetClientsByNameQuery, IEnumerable<DetailClientDTO>>.Handle(GetClientsByNameQuery request,
                                                                                                                       CancellationToken cancellationToken)
  {
    var clients = await _unitOfWork.Clients
                                   .GetAllByNameWithChildrensAsync(request.Name,
                                                                   cancellationToken);

    if (!clients.Any())
      throw new EntityNotFoundException($"Name = {request.Name}");

    var result = _mapper.Map<IEnumerable<DetailClientDTO>>(clients);

    var clientIds = result.Select(x => x.IdClient).ToList();

    var sprVlanLs = await _unitOfWork.SPRVlans.GetAllByClientIdsAsync(clientIds, cancellationToken);

    var vlans = _mapper.Map<IEnumerable<SPRVlanDTO>>(sprVlanLs);

    foreach (var vlan in vlans)
    {
      var client = result.First(x => x.IdClient == vlan.IdClient);
      client.SPRVlans.Add(vlan);
    }

    return result;
  }
}

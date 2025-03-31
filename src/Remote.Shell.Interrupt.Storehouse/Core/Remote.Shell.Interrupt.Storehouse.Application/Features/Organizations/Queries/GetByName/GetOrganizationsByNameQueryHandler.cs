namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetByName;

public record GetClientsCODByNameQuery(string Name) : IQuery<IEnumerable<ClientCODDTODetail>>;

internal class GetClientsCODByNameQueryHandler(IUnitOfWork unitOfWork,
                                               IMapper mapper)
  : IQueryHandler<GetClientsCODByNameQuery, IEnumerable<ClientCODDTODetail>>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<IEnumerable<ClientCODDTODetail>> IRequestHandler<GetClientsCODByNameQuery, IEnumerable<ClientCODDTODetail>>.Handle(GetClientsCODByNameQuery request,
                                                                                                                                CancellationToken cancellationToken)
  {
    var clients = await _unitOfWork.ClientCODLs
                                   .GetAllByNameWithChildrensAsync(request.Name,
                                                                   cancellationToken);

    if (!clients.Any())
      throw new EntityNotFoundException($"Name = {request.Name}");

    var result = _mapper.Map<IEnumerable<ClientCODDTODetail>>(clients);

    var clientIds = result.Select(x => x.IdClient).ToList();

    var sprVlanLs = await _unitOfWork.SPRVlanLs.GetAllByIdsAsync(clientIds, cancellationToken);

    var vlans = _mapper.Map<IEnumerable<SPRVlanDTO>>(sprVlanLs);

    foreach (var vlan in vlans)
    {
      var client = result.First(x => x.IdClient == vlan.IdClient);
      client.SPRVlans.Add(vlan);
    }

    return result;
  }
}

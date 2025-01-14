namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetAll;

public record GetClientsCODQuery() : IQuery<IEnumerable<ClientCODDTO>>;

internal class GetClientsCODQueryHandler(IUnitOfWork unitOfWork,
                                         IMapper mapper)
  : IQueryHandler<GetClientsCODQuery, IEnumerable<ClientCODDTO>>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<IEnumerable<ClientCODDTO>> IRequestHandler<GetClientsCODQuery, IEnumerable<ClientCODDTO>>.Handle(GetClientsCODQuery request,
                                                                                                              CancellationToken cancellationToken)
  {
    var clients = await _unitOfWork.ClientCODLs
                                   .GetAllWithChildrensAsync(cancellationToken);

    var sprVlanLs = await _unitOfWork.SPRVlanLs.GetAllAsync(cancellationToken);

    var vlans = _mapper.Map<List<SPRVlanDTO>>(sprVlanLs);

    var result = _mapper.Map<List<ClientCODDTO>>(clients);

    foreach (var vlan in vlans)
    {
      var client = result.First(x => x.IdClient == vlan.IdClient);
      client.SPRVlans.Add(vlan);
    }

    return result;
  }
}

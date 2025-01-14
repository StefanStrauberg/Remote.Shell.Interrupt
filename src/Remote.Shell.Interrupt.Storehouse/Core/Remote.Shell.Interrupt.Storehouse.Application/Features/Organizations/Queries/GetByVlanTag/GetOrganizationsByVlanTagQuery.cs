namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetByVlanTag;

public record GetClientsCODByVlanTagQuery(int VlanTag) : IQuery<IEnumerable<ClientCODDTO>>;

internal class GetClientsCODByVlanTagQueryHandler(IUnitOfWork unitOfWork,
                                                  IMapper mapper)
  : IQueryHandler<GetClientsCODByVlanTagQuery, IEnumerable<ClientCODDTO>>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<IEnumerable<ClientCODDTO>> IRequestHandler<GetClientsCODByVlanTagQuery, IEnumerable<ClientCODDTO>>.Handle(GetClientsCODByVlanTagQuery request,
                                                                                                                       CancellationToken cancellationToken)
  {
    // Проверка существования назначения влан с ID
    var existingVlan = await _unitOfWork.SPRVlanLs
                                        .AnyByVlanTagAsync(request.VlanTag,
                                                          cancellationToken);

    // Если алвн не найдено — исключение
    if (!existingVlan)
      throw new EntityNotFoundById(typeof(SPRVlanL),
                                   request.VlanTag.ToString());

    var vlan = await _unitOfWork.SPRVlanLs
                                .GetByVlanTagAsync(request.VlanTag,
                                                   cancellationToken);
    var clientName = await _unitOfWork.ClientCODLs
                                      .GetClientNameByClientIdAsync(vlan.IdClient,
                                                                    cancellationToken);

    if (clientName is null)
      throw new EntityNotFoundException($"VlanTag = {request.VlanTag}");

    var name = ExtractNameInQuotes(clientName);
    var clients = await _unitOfWork.ClientCODLs
                                   .GetAllByNameAsync(name,
                                                      cancellationToken);
    var result = _mapper.Map<IEnumerable<ClientCODDTO>>(clients);
    var clientIds = result.Select(x => x.IdClient).ToList();
    var sprVlanLs = await _unitOfWork.SPRVlanLs.GetAllByIdsAsync(clientIds, cancellationToken);
    var vlans = _mapper.Map<IEnumerable<SPRVlanDTO>>(sprVlanLs);

    foreach (var item in vlans)
    {
      var client = result.First(x => x.IdClient == item.IdClient);
      client.SPRVlans.Add(item);
    }

    return result;
  }

  static string ExtractNameInQuotes(string input)
  {
    var regex = new Regex(@"^(.*?)\s*\(");
    var match = regex.Match(input);

    if (match.Success)
    {
      return match.Groups[1].Value;
    }

    return input;
  }
}
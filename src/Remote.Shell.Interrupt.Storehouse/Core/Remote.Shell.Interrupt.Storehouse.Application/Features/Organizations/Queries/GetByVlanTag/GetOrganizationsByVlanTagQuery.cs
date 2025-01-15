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

    var clientIds = await _unitOfWork.SPRVlanLs
                                     .GetClientsIdsByVlantTag(request.VlanTag,
                                                              cancellationToken);

    var clientNames = await _unitOfWork.ClientCODLs
                                       .GetClientsNamesByClientIdsAsync(clientIds,
                                                                        cancellationToken);
    var names = ExtractNameInQuotes(clientNames);

    var namesAreEquals = AllStringsAreEqual(names);

    IEnumerable<ClientCODL> clients;

    if (namesAreEquals)
    {
      clients = await _unitOfWork.ClientCODLs
                                 .GetAllByNameAsync(names.First(),
                                                    cancellationToken);
    }
    else
    {
      clients = await _unitOfWork.ClientCODLs
                                 .GetAllByNamesAsync(names,
                                                     cancellationToken);
    }

    var result = _mapper.Map<IEnumerable<ClientCODDTO>>(clients);
    var ids = result.Select(x => x.IdClient).ToList();
    var sprVlanLs = await _unitOfWork.SPRVlanLs.GetAllByIdsAsync(ids, cancellationToken);
    var vlans = _mapper.Map<IEnumerable<SPRVlanDTO>>(sprVlanLs);

    foreach (var vlan in vlans)
    {
      var client = result.First(x => x.IdClient == vlan.IdClient);
      client.SPRVlans.Add(vlan);
    }

    return result;
  }
  static bool AllStringsAreEqual(IEnumerable<string> strings)
  {
    // Получаем первую строку для сравнения
    string firstString = strings.First();

    // Проверяем, все ли остальные строки одинаковы с первой
    return strings.All(s => s == firstString);
  }

  static List<string> ExtractNameInQuotes(IEnumerable<string> inputs)
  {
    var result = new List<string>();

    foreach (var input in inputs)
    {
      var regex = new Regex(@"^(.*?)\s*\(");
      var match = regex.Match(input);

      if (match.Success)
      {
        result.Add(match.Groups[1].Value);
      }
    }

    return result;
  }
}
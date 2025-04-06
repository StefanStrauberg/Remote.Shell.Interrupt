namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetByVlanTag;

public record GetClientsByVlanTagQuery(int VlanTag) : IQuery<IEnumerable<DetailClientDTO>>;

internal partial class GetClientsByVlanTagQueryHandler(IUnitOfWork unitOfWork,
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
    // Проверка существования назначения влан с ID
    var existingVlan = await _unitOfWork.SPRVlans
                                        .AnyByVlanTagAsync(request.VlanTag,
                                                          cancellationToken);

    // Если алвн не найдено — исключение
    if (!existingVlan)
      throw new EntityNotFoundById(typeof(SPRVlan),
                                   request.VlanTag.ToString());

    var clientIds = await _unitOfWork.SPRVlans
                                     .GetClientsIdsByVlantTag(request.VlanTag,
                                                              cancellationToken);

    // TODO
    var clientNames = new List<string>();

    foreach (var client in clientIds)
    {
      var tmps = await _unitOfWork.Clients
                                  .GetAllShortClientsAsync(new RequestParameters() 
                                                           { 
                                                             Filters = $"IdClient==\"{client}\""
                                                           },
                                                           cancellationToken);
      foreach (var tmp in tmps)
        clientNames.Add(tmp.Name);
    }

    var names = ExtractNameInQuotes(clientNames);

    var namesAreEquals = AllStringsAreEqual(names);

    IEnumerable<Client> clients = [];

    if (namesAreEquals)
    {
      clients = await _unitOfWork.Clients
                                 .GetAllShortClientsAsync(new RequestParameters()
                                                          {
                                                            Filters = $"name~=\"{names.First()}\""
                                                          },
                                                          cancellationToken);
    }
    else
    {
      foreach (var name in names)
      {
        var tmps = await _unitOfWork.Clients
                                    .GetAllShortClientsAsync(new RequestParameters() 
                                                             { 
                                                               Filters = $"Name==\"{name}\""
                                                             },
                                                             cancellationToken);
        clients = tmps;
      }
    }

    var result = _mapper.Map<IEnumerable<DetailClientDTO>>(clients);
    var ids = result.Select(x => x.IdClient).ToList();
    var sprVlanLs = await _unitOfWork.SPRVlans.GetAllByClientIdsAsync(ids, cancellationToken);
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
      var regex = MyRegex();
      var match = regex.Match(input);

      if (match.Success)
      {
        result.Add(match.Groups[1].Value);
      }
    }

    return result;
  }

  [GeneratedRegex(@"^(.*?)\s*\(")]
  private static partial Regex MyRegex();
}
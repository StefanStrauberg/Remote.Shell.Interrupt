namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetByVlanTag;

// Запрос получения клиентов по VLAN Tag.
public record GetClientsByVlanTagQuery(int VlanTag) : IQuery<IEnumerable<DetailClientDTO>>;

internal class GetClientsByVlanTagQueryHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                               IMapper mapper)
  : IQueryHandler<GetClientsByVlanTagQuery, IEnumerable<DetailClientDTO>>
{
  async Task<IEnumerable<DetailClientDTO>> IRequestHandler<GetClientsByVlanTagQuery, IEnumerable<DetailClientDTO>>.Handle(GetClientsByVlanTagQuery request,
                                                                                                                          CancellationToken cancellationToken)
  {
    // Проверяем корректность значения VLAN Tag.
    if (request.VlanTag == 0)
      throw new ArgumentException("Invalid VLAN Tag.", nameof(request.VlanTag));

    // Формируем запрос для получения данных VLAN.
    var getSPRVlansQuery = new GetSPRVlansQuery(new RequestParameters()
                                                {
                                                  Filters = $"IdVlan=={request.VlanTag}"
                                                });
    
    // Инициализируем обработчик запроса для SPR VLAN.
    var getSPRVlansQueryHandler = new GetSPRVlansQueryHandler(locBillUnitOfWork, mapper);

    var sprVlans = await ((IRequestHandler<GetSPRVlansQuery, PagedList<SPRVlanDTO>>)getSPRVlansQueryHandler).Handle(getSPRVlansQuery,
                                                                                                                    cancellationToken);

    // Создаем коллекцию клиентов.
    HashSet<Client> clients = [];

    foreach (var item in sprVlans)
    {
      // Извлекаем данные клиента из базы.
      var client = await locBillUnitOfWork.Clients
                                          .GetOneWithChildrensAsync(new RequestParameters()
                                                                    {
                                                                      Filters = $"IdClient=={item.IdClient}"
                                                                    }, 
                                                                    cancellationToken);
      clients.Add(client);
    }

    // Проверка всех ли клиентов извлекли
    var uniqName = ClientNameHelper.ExtractUniqName([.. clients.Select(x => x.Name)]);

    if (!string.IsNullOrEmpty(uniqName))
    {
      var additionalClients = await locBillUnitOfWork.Clients
                                                     .GetManyWithChildrenAsync(new RequestParameters()
                                                     {
                                                       Filters = $"Name~={uniqName}"
                                                     }, 
                                                     cancellationToken);
      // TODO
    }

    // Преобразуем данные клиентов в DTO.
    var result = mapper.Map<IEnumerable<DetailClientDTO>>(clients);

    // Возвращаем преобразованные данные.
    return result;
  }
}
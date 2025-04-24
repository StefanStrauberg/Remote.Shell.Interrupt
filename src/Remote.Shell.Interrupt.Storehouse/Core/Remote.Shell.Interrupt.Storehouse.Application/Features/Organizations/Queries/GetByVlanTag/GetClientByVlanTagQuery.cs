namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetByVlanTag;

// Черт возьми, запрос на получение клиентов по VLAN Tag... надеюсь, это не сломает мультивселенную.
public record GetClientsByVlanTagQuery(int VlanTag) : IQuery<IEnumerable<DetailClientDTO>>;

internal class GetClientsByVlanTagQueryHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                               IMapper mapper)
  : IQueryHandler<GetClientsByVlanTagQuery, IEnumerable<DetailClientDTO>>
{
  async Task<IEnumerable<DetailClientDTO>> IRequestHandler<GetClientsByVlanTagQuery, IEnumerable<DetailClientDTO>>.Handle(GetClientsByVlanTagQuery request,
                                                                                                                          CancellationToken cancellationToken)
  {
    // VLAN Tag равен нулю? В смысле нулю? Это ошибка, так что бросай исключение и забудь об этом!
    if (request.VlanTag == 0)
      throw new ArgumentException("Invalid VLAN Tag.", nameof(request.VlanTag));

    // Фильтры, фильтры, фильтры. Всегда фильтры. Кто вообще это придумал? Ладно, создаем запрос.
    var getSPRVlansQuery = new GetSPRVlansQuery(new RequestParameters()
                                                {
                                                  Filters = $"IdVlan=={request.VlanTag}"
                                                });
    
    // Запрос к SPR Vlans? Надеюсь, они не аномальные данные, которые уничтожат вселенную!
    var getSPRVlansQueryHandler = new GetSPRVlansQueryHandler(locBillUnitOfWork, mapper);

    var sprVlans = await ((IRequestHandler<GetSPRVlansQuery, PagedList<SPRVlanDTO>>)getSPRVlansQueryHandler).Handle(getSPRVlansQuery,
                                                                                                                    cancellationToken);

    // Хешсет клиентов? А, да, делаем это старомодным, хаотичным способом!
    HashSet<Client> clients = [];

    foreach (var item in sprVlans)
    {
      // Получаем клиента. Надеюсь, это не очередной космический мошенник!
      var client = await locBillUnitOfWork.Clients
                                          .GetOneWithChildrensAsync(new RequestParameters()
                                                                    {
                                                                      Filters = $"IdClient=={item.IdClient}"
                                                                    }, 
                                                                    cancellationToken);
      clients.Add(client);
    }

    // Ура, преобразование в DTO... или что-то такое.
    var result = mapper.Map<IEnumerable<DetailClientDTO>>(clients);

    // Возвращаем результат. А если он не работает, просто притворись, что ты не видел!
    return result;
  }
}
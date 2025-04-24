namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetByVlanTag;

// О, черт, запрос на VLAN Tag! Надеюсь, это не очередная галактическая катастрофа.
public record GetNetworkDeviceByVlanTagQuery(int VlanTag) : IQuery<CompoundObjectDTO>;

internal class GetNetworkDeviceByVlanTagQueryHandler(INetDevUnitOfWork netDevUnitOfWork,
                                                     ILocBillUnitOfWork locBillUnitOfWork,
                                                     IMapper mapper)
  : IQueryHandler<GetNetworkDeviceByVlanTagQuery, CompoundObjectDTO>
{
  async Task<CompoundObjectDTO> IRequestHandler<GetNetworkDeviceByVlanTagQuery, CompoundObjectDTO>.Handle(GetNetworkDeviceByVlanTagQuery request,
                                                                                                          CancellationToken cancellationToken)
  {
    // VLAN Tag равен нулю? Это что, шутка? Исправь это, пока вселенная не рухнула!
    if (request.VlanTag == 0)
      throw new ArgumentException("Invalid VLAN Tag.", nameof(request.VlanTag));

    // Ладно, создаем запрос для клиентов. Надеюсь, они не окажутся инопланетными паразитами.
    var getClientsByVlanTagQuery = new GetClientsByVlanTagQuery(request.VlanTag);

    // Обработчик запросов? Да это же просто очередной бюрократический кошмар!
    var getClientsByVlanTagQueryHandler = new GetClientsByVlanTagQueryHandler(locBillUnitOfWork,
                                                                              mapper);

    // Извлекаем клиентов. Если они начнут жаловаться, я тут ни при чем!
    var clients = await ((IRequestHandler<GetClientsByVlanTagQuery, IEnumerable<DetailClientDTO>>)getClientsByVlanTagQueryHandler).Handle(getClientsByVlanTagQuery,
                                                                                                                                          cancellationToken);
    
    // Список сетевых устройств? Пустой, как мои запасы межгалактического топлива.
    List<NetworkDevice> networkDevices = [];

    // Собираем VLAN теги. Надеюсь, они не взорвутся.
    var vlanTags = clients.SelectMany(x => x.SPRVlans)
                          .Select(x => x.IdVlan);

    // Добавляем устройства. Если что-то пойдет не так, я сваливаю!
    networkDevices.AddRange(await netDevUnitOfWork.NetworkDevices
                                                  .GetManyWithChildrenByVlanTagsAsync(vlanTags,
                                                                                      cancellationToken));

    // Очистим порты. Или не очистим. Кто вообще знает, что тут происходит?
    PrepareAndCleanAggregationPorts.Handle(networkDevices);

    // Финальный результат. Надеюсь, он не вызовет межгалактический хаос.
    var reuslt = new CompoundObjectDTO()
    {
      NetworkDevices = mapper.Map<IEnumerable<NetworkDeviceDTO>>(networkDevices),
      Clients = clients
    };

    // Возвращаем результат. Если он не работает, это не моя вина!
    return reuslt;
  }
}

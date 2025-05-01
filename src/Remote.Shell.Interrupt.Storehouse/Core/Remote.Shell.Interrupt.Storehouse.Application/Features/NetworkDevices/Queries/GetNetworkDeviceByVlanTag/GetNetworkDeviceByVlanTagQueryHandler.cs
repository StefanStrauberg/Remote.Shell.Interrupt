namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetNetworkDeviceByVlanTag;

// Запрос получения сетевого устройства по VLAN Tag.
public record GetNetworkDeviceByVlanTagQuery(int VlanTag) : IQuery<CompoundObjectDTO>;

internal class GetNetworkDeviceByVlanTagQueryHandler(INetDevUnitOfWork netDevUnitOfWork,
                                                     ILocBillUnitOfWork locBillUnitOfWork,
                                                     INetworkDeviceSpecification networkDeviceSpecification,
                                                     ISPRVlanSpecification sPRVlanSpecification,
                                                     IClientSpecification clientSpecification,
                                                     IQueryFilterParser queryFilterParser,
                                                     IMapper mapper)
  : IQueryHandler<GetNetworkDeviceByVlanTagQuery, CompoundObjectDTO>
{
  async Task<CompoundObjectDTO> IRequestHandler<GetNetworkDeviceByVlanTagQuery, CompoundObjectDTO>.Handle(GetNetworkDeviceByVlanTagQuery request,
                                                                                                          CancellationToken cancellationToken)
  {
    // Проверяем, что VLAN Tag задан корректно.
    if (request.VlanTag == 0)
      throw new ArgumentException("Invalid VLAN Tag.", nameof(request.VlanTag));

    // Создаем запрос для получения клиентов по VLAN Tag.
    var getClientsByVlanTagQuery = new GetClientsByVlanTagQuery(request.VlanTag);

    // Инициализируем обработчик запросов для клиентов.
    var getClientsByVlanTagQueryHandler = new GetClientsByVlanTagQueryHandler(locBillUnitOfWork,
                                                                              sPRVlanSpecification,
                                                                              clientSpecification,
                                                                              queryFilterParser,
                                                                              mapper);

    // Извлекаем список клиентов.
    var clients = await ((IRequestHandler<GetClientsByVlanTagQuery, IEnumerable<DetailClientDTO>>)getClientsByVlanTagQueryHandler).Handle(getClientsByVlanTagQuery,
                                                                                                                                          cancellationToken);
    
    // Инициализируем список сетевых устройств.
    List<NetworkDevice> networkDevices = [];

    // Извлекаем VLAN теги из данных клиентов.
    var vlanTags = clients.SelectMany(x => x.SPRVlans)
                          .Select(x => x.IdVlan)
                          .Distinct();

    var requestParameters = new RequestParameters
    {
      Filters = [
        new ()
        {
          PropertyPath = "PortsOfNetworkDevice.VLANs.VLANTag",
          Operator = FilterOperator.Equals,
          Value = request.VlanTag.ToString(),
        }
      ]
    };

    // Parse filter
    var filterExpr = queryFilterParser.ParseFilters<NetworkDevice>(requestParameters.Filters);

    // Build base specification
    var baseSpec = BuildSpecification(networkDeviceSpecification,
                                      filterExpr);

    // Извлекаем устройства, связанные с VLAN тегами.
    networkDevices.AddRange(await netDevUnitOfWork.NetworkDevices
                                                  .GetManyWithChildrenAsync(baseSpec,
                                                                            cancellationToken));

    // Заполнение агрегированных портов
    foreach (var networkDevice in networkDevices)
    {
        // Извлекаем родительские порты и материализуем их в список
        var parentPorts = networkDevice.PortsOfNetworkDevice
                                      .Where(port => port.ParentPortId is null);

        // Если родительских портов нет, переходим к следующему устройству.
        if (!parentPorts.Any())
            continue;

        // Извлекаем идентификаторы родительских портов
        var parentPortsIds = parentPorts.Select(port => port.Id);

        // Асинхронно получаем все дочерние порты для данных родительских портов.
        var children = await netDevUnitOfWork.Ports
                                             .GetAllAggregatedPortsByListAsync(parentPortsIds, cancellationToken);

        // Группируем дочерние порты, гарантируя, что ParentPortId имеет значение
        var childrenByParent = children.Where(child => child.ParentPortId.HasValue)         // Отфильтровываем элементы с не null ParentPortId
                                       .GroupBy(child => child.ParentPortId!.Value)            // Используем Value для получения Guid
                                       .ToDictionary(group => group.Key, group => group.ToArray());


        // Записываем дочерние порты каждому родительскому порту
        foreach (var port in parentPorts)
        {
            port.AggregatedPorts = childrenByParent.TryGetValue(port.Id, out var aggregated)
                                  ? aggregated
                                  : []; // Если для родителя нет детей, устанавливаем пустой список.
        }
    }


    // Очищаем и подготавливаем данные портов.
    PrepareAndCleanAggregationPorts.Handle(networkDevices);

    // Формируем финальный результат.
    var result = new CompoundObjectDTO()
    {
      NetworkDevices = mapper.Map<IEnumerable<NetworkDeviceDTO>>(networkDevices),
      Clients = clients
    };

    // Возвращаем результат.
    return result;
  }

  static INetworkDeviceSpecification BuildSpecification(INetworkDeviceSpecification baseSpec,
                                                        Expression<Func<NetworkDevice, bool>>? filterExpr)
  {
    var spec = baseSpec.AddInclude(x => x.PortsOfNetworkDevice);
    // TODO thenInclude...

    if (filterExpr is not null)
        spec.AddFilter(filterExpr);

    return (INetworkDeviceSpecification)spec;
  }
}

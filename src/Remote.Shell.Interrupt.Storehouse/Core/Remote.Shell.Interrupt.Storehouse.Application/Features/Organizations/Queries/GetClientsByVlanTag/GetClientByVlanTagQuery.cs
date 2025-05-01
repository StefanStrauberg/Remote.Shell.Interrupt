namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetClientsByVlanTag;

// Запрос получения клиентов по VLAN Tag.
public record GetClientsByVlanTagQuery(int VlanTag) : IQuery<IEnumerable<DetailClientDTO>>;

internal class GetClientsByVlanTagQueryHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                               ISPRVlanSpecification sPRVlanSpecification,
                                               IClientSpecification clientSpecification,
                                               IQueryFilterParser queryFilterParser,
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
    var requestParameters = new RequestParameters
    {
      Filters = [
        new ()
        {
          PropertyPath = nameof(SPRVlan.IdVlan),
          Operator = FilterOperator.Equals,
          Value = request.VlanTag.ToString()
        }
      ]
    };

    // Формируем запрос для получения данных VLAN.
    var getSPRVlansByFilterQuery = new GetSPRVlansByFilterQuery(requestParameters);
    
    // Инициализируем обработчик запроса для SPR VLAN.
    var getSPRVlansByFilterQueryHandler = new GetSPRVlansByFilterQueryHandler(locBillUnitOfWork,
                                                                              sPRVlanSpecification,
                                                                              queryFilterParser,
                                                                              mapper);

    var sprVlans = await ((IRequestHandler<GetSPRVlansByFilterQuery, PagedList<SPRVlanDTO>>)getSPRVlansByFilterQueryHandler).Handle(getSPRVlansByFilterQuery,
                                                                                                                                    cancellationToken);

    // Создаем коллекцию клиентов.
    HashSet<Client> clients = [];

    foreach (var item in sprVlans)
    {
      requestParameters.Filters[0].PropertyPath = nameof(Client.IdClient);
      requestParameters.Filters[0].Value = item.IdClient.ToString();

      // Parse filter
      var filterExpr = queryFilterParser.ParseFilters<Client>(requestParameters.Filters);
      
      // Build base specification
      var baseSpec = BuildSpecification(clientSpecification, filterExpr);

      // Извлекаем данные клиента из базы.
      var client = await locBillUnitOfWork.Clients
                                          .GetOneWithChildrenAsync(baseSpec, 
                                                                   cancellationToken);
      clients.Add(client);
    }

    // // Проверка всех ли клиентов извлекли
    // var uniqName = ClientNameHelper.ExtractUniqName([.. clients.Select(x => x.Name)]);

    // if (!string.IsNullOrEmpty(uniqName))
    // {
    //   var additionalClients = await locBillUnitOfWork.Clients
    //                                                  .GetManyWithChildrenAsync(new RequestParameters()
    //                                                  {
    //                                                    Filters = $"Name~={uniqName}"
    //                                                  }, 
    //                                                  cancellationToken);
    //   // TODO
    // }

    // Преобразуем данные клиентов в DTO.
    var result = mapper.Map<IEnumerable<DetailClientDTO>>(clients);

    // Возвращаем преобразованные данные.
    return result;
  }

  /// <summary>
  /// Builds the specification with included entities and filters.
  /// </summary>
  /// <param name="baseSpec">The base client specification.</param>
  /// <param name="filterExpr">The filter expression to apply.</param>
  /// <returns>
  /// An updated client specification including related entities and filters.
  /// </returns>
  static IClientSpecification BuildSpecification(IClientSpecification baseSpec,
                                                 Expression<Func<Client, bool>>? filterExpr)
  {
    var spec = baseSpec.AddInclude(c => c.COD)
                       .AddInclude(c => c.TfPlan!)
                       .AddInclude(c => c.SPRVlans);

    if (filterExpr is not null)
        spec.AddFilter(filterExpr);

    return (IClientSpecification)spec;
  }
}

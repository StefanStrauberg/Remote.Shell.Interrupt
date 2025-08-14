namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetClientsByVlanTag;

public record GetClientsByVlanTagQuery(int VlanTag) 
  : FindEntitiesByFilterQuery<IEnumerable<DetailClientDTO>>(new RequestParameters
  {
    Filters = [
      new ()
      {
        PropertyPath = nameof(SPRVlan.IdVlan),
        Operator = FilterOperator.Equals,
        Value = VlanTag.ToString()
      }
    ]
  });

internal class GetClientsByVlanTagQueryHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                               ISPRVlanSpecification sPRVlanSpecification,
                                               IClientSpecification clientSpecification,
                                               IQueryFilterParser queryFilterParser,
                                               IMapper mapper)
  : FindEntitiesByFilterQueryHandler<Client, DetailClientDTO, GetClientsByVlanTagQuery>(clientSpecification, queryFilterParser, mapper)
{
  // async Task<IEnumerable<DetailClientDTO>> IRequestHandler<GetClientsByVlanTagQuery, IEnumerable<DetailClientDTO>>.Handle(GetClientsByVlanTagQuery request,
  //                                                                                                                         CancellationToken cancellationToken)
  // {
  // // Проверяем корректность значения VLAN Tag.
  // if (request.VlanTag == 0)
  //   throw new ArgumentException("Invalid VLAN Tag.", nameof(request.VlanTag));

  // // Формируем запрос для получения данных VLAN.
  // var requestParameters = new RequestParameters
  // {
  //   Filters = [
  //     new ()
  //     {
  //       PropertyPath = nameof(SPRVlan.IdVlan),
  //       Operator = FilterOperator.Equals,
  //       Value = request.VlanTag.ToString()
  //     }
  //   ]
  // };

  // // Формируем запрос для получения данных VLAN.
  // var getSPRVlansByFilterQuery = new GetSPRVlansByFilterQuery(requestParameters);

  // // Инициализируем обработчик запроса для SPR VLAN.
  // var getSPRVlansByFilterQueryHandler = new GetSPRVlansByFilterQueryHandler(locBillUnitOfWork,
  //                                                                           sPRVlanSpecification,
  //                                                                           queryFilterParser,
  //                                                                           mapper);

  // var sprVlans = await ((IRequestHandler<GetSPRVlansByFilterQuery, PagedList<SPRVlanDTO>>)getSPRVlansByFilterQueryHandler).Handle(getSPRVlansByFilterQuery,
  //                                                                                                                                 cancellationToken);

  // // Создаем коллекцию клиентов.
  // HashSet<Client> clients = [];

  // foreach (var item in sprVlans)
  // {
  //   requestParameters.Filters[0].PropertyPath = nameof(Client.IdClient);
  //   requestParameters.Filters[0].Value = item.IdClient.ToString();

  //   // Parse filter
  //   var filterExpr = queryFilterParser.ParseFilters<Client>(requestParameters.Filters);

  //   // Build base specification
  //   var baseSpec = BuildSpecification(clientSpecification, filterExpr);

  //   // Извлекаем данные клиента из базы.
  //   var client = await locBillUnitOfWork.Clients
  //                                       .GetOneWithChildrenAsync(baseSpec, 
  //                                                                cancellationToken);
  //   clients.Add(client);
  // }

  // // Преобразуем данные клиентов в DTO.
  // var result = mapper.Map<IEnumerable<DetailClientDTO>>(clients);

  // // Возвращаем преобразованные данные.
  // return result;
  //   return await Task.FromResult<IEnumerable<DetailClientDTO>>([]);
  // }

  // static IClientSpecification BuildSpecification(IClientSpecification baseSpec,
  //                                                Expression<Func<Client, bool>>? filterExpr)
  // {
  //   var spec = baseSpec.AddInclude(c => c.COD)
  //                      .AddInclude(c => c.TfPlan!)
  //                      .AddInclude(c => c.SPRVlans);

  //   if (filterExpr is not null)
  //       spec.AddFilter(filterExpr);

  //   return (IClientSpecification)spec;
  // }
  protected override Task<int> CountResultsAsync(ISpecification<Client> specification, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  protected override Task<IEnumerable<Client>> FetchEntitiesAsync(ISpecification<Client> specification, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}

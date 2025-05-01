namespace Remote.Shell.Interrupt.Storehouse.Application.Features.SPRVlans.Queries.GetSPRVlansByFilter;

// Запрос для получения списка SPR VLAN.
public record GetSPRVlansByFilterQuery(RequestParameters RequestParameters) 
  : IQuery<PagedList<SPRVlanDTO>>;

internal class GetSPRVlansByFilterQueryHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                               ISPRVlanSpecification specification,
                                               IQueryFilterParser queryFilterParser,
                                               IMapper mapper) 
  : IQueryHandler<GetSPRVlansByFilterQuery, PagedList<SPRVlanDTO>>
{
  async Task<PagedList<SPRVlanDTO>> IRequestHandler<GetSPRVlansByFilterQuery, PagedList<SPRVlanDTO>>.Handle(GetSPRVlansByFilterQuery request,
                                                                                                            CancellationToken cancellationToken)
  {
    // Parse filter
    var filterExpr = queryFilterParser.ParseFilters<SPRVlan>(request.RequestParameters
                                                                    .Filters);

    // Build base specification
    var baseSpec = BuildSpecification(specification,
                                      filterExpr);

    // Count records (without pagination)
    var countSpec = (ISPRVlanSpecification)baseSpec.Clone();
    var count = await locBillUnitOfWork.SPRVlans
                                       .GetCountAsync(countSpec,
                                                      cancellationToken);

    // Pagination parameters
    var pageNumber = request.RequestParameters.PageNumber ?? 0;
    var pageSize = request.RequestParameters.PageSize ?? 0;

    if (request.RequestParameters.EnablePagination)
        baseSpec.WithPagination(pageNumber,
                                pageSize);

    // Загружаем список SPR VLAN из базы данных.
    var sprVlans = await locBillUnitOfWork.SPRVlans
                                          .GetManyShortAsync(baseSpec,
                                                             cancellationToken);
                                                             
    // Если данные отсутствуют, возвращаем пустой объект.
    if (!sprVlans.Any())
      return new PagedList<SPRVlanDTO>([],0,0,0);

    // Преобразуем данные SPR VLAN в формат DTO.
    var result = mapper.Map<List<SPRVlanDTO>>(sprVlans);

    // Return results
    return new PagedList<SPRVlanDTO>(result,
                                     count,
                                     pageNumber,
                                     pageSize);
  }

  static ISPRVlanSpecification BuildSpecification(ISPRVlanSpecification baseSpec,
                                                  Expression<Func<SPRVlan, bool>>? filterExpr)
  {
    var spec = baseSpec;

    if (filterExpr is not null)
        spec.AddFilter(filterExpr);

    return spec;
  }
}

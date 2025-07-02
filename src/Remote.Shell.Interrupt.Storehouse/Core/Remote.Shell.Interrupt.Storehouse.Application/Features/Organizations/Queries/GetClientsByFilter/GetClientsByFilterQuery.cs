namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetClientsByFilter;

/// <summary>
/// Represents a query to fetch clients based on filtering criteria.
/// </summary>
/// <param name="RequestParameters">The request parameters containing filtering and pagination settings.</param>
public record GetClientsByFilterQuery(RequestParameters RequestParameters) 
  : IQuery<PagedList<ShortClientDTO>>;

/// <summary>
/// Handles the GetClientsByFilterQuery and retrieves filtered clients.
/// </summary>
/// <remarks>
/// This handler processes filtering expressions, builds the necessary specifications,
/// applies pagination, and retrieves the resulting clients.
/// </remarks>
/// <param name="locBillUnitOfWork">Unit of work for database operations.</param>
/// <param name="specification">Client specification used for filtering.</param>
/// <param name="queryFilterParser">Parser for processing filter expressions.</param>
/// <param name="mapper">Object mapper for DTO transformation.</param>
internal class GetClientsByFilterQueryHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                              IClientSpecification specification,
                                              IQueryFilterParser queryFilterParser,
                                              IMapper mapper)
  : IQueryHandler<GetClientsByFilterQuery, PagedList<ShortClientDTO>>
{
  /// <summary>
  /// Handles the request to retrieve clients based on filter conditions.
  /// </summary>
  /// <param name="request">The query request containing filtering and pagination parameters.</param>
  /// <param name="cancellationToken">Token for request cancellation support.</param>
  /// <returns>A paged list of short client DTOs.</returns>
  async Task<PagedList<ShortClientDTO>> IRequestHandler<GetClientsByFilterQuery, PagedList<ShortClientDTO>>.Handle(GetClientsByFilterQuery request,
                                                                                                                   CancellationToken cancellationToken)
  {
    // // Parse the filter expression
    // var filterExpr = queryFilterParser.ParseFilters<Client>(request.RequestParameters
    //                                                                .Filters);

    // // Build the base specification with filtering applied
    // var baseSpec = BuildSpecification(specification,
    //                                   filterExpr);

    // // Create a specification for counting total matching records
    // var countSpec = baseSpec.Clone();

    // // Extract pagination parameters
    // var pageNumber = request.RequestParameters.PageNumber ?? 0;
    // var pageSize = request.RequestParameters.PageSize ?? 0;

    // // Apply pagination settings if enabled
    // if (request.RequestParameters.IsPaginated)
    //     baseSpec.ConfigurePagination(pageNumber,
    //                             pageSize);

    // // Retrieve data
    // var clients = await locBillUnitOfWork.Clients
    //                                      .GetManyShortAsync(baseSpec,
    //                                                         cancellationToken);

    // // Return an empty paginated list if no data are found.
    // if (!clients.Any())
    //   return new PagedList<ShortClientDTO>([],0,0,0);

    // // Retrieve the total count of matching records
    // var count = await locBillUnitOfWork.Clients
    //                                    .GetCountAsync(countSpec,
    //                                                   cancellationToken);

    // // Map the retrieved data to the DTO
    // var result = mapper.Map<IEnumerable<ShortClientDTO>>(clients);

    // // Return the mapped result
    // return new PagedList<ShortClientDTO>(result,
    //                                      count,
    //                                      pageNumber,
    //                                      pageSize);
    return await Task.FromResult<PagedList<ShortClientDTO>>(new PagedList<ShortClientDTO>([],0,new(0,0)));
  }

  /// <summary>
  /// Builds the specification by applying the provided filter expression.
  /// </summary>
  /// <param name="baseSpec">The base client specification.</param>
  /// <param name="filterExpr">The filter expression to apply.</param>
  /// <returns>An updated specification with filtering applied.</returns>
  // static IClientSpecification BuildSpecification(IClientSpecification baseSpec,
  //                                                Expression<Func<Client, bool>>? filterExpr)
  // {
  //   var spec = baseSpec;

  //   if (filterExpr is not null)
  //       spec.AddFilter(filterExpr);

  //   return spec;
  // }
}

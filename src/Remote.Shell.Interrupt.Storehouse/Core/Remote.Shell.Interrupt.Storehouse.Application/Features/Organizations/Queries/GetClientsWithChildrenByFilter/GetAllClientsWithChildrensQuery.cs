namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetClientsWithChildrenByFilter;

public record GetClientsWithChildrenByFilterQuery(RequestParameters RequestParameters) 
  : IQuery<PagedList<DetailClientDTO>>;

internal class GetClientsWithChildrenByFilterQueryHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                                          IClientSpecification specification,
                                                          IQueryFilterParser queryFilterParser,
                                                          IMapper mapper)
  : IQueryHandler<GetClientsWithChildrenByFilterQuery, PagedList<DetailClientDTO>>
{
  async Task<PagedList<DetailClientDTO>> IRequestHandler<GetClientsWithChildrenByFilterQuery, PagedList<DetailClientDTO>>.Handle(GetClientsWithChildrenByFilterQuery request,
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

    // // Retrieve data, including related entities
    // var clients = await locBillUnitOfWork.Clients
    //                                      .GetManyWithChildrenAsync(baseSpec,
    //                                                                cancellationToken);

    // // Return an empty paginated list if no data are found.
    // if (!clients.Any())
    //   return new PagedList<DetailClientDTO>([],0,0,0);

    // // Retrieve the total count of matching records
    // var count = await locBillUnitOfWork.Clients
    //                                    .GetCountAsync(countSpec,
    //                                                   cancellationToken);

    // // Map the retrieved data to the DTO
    // var result = mapper.Map<IEnumerable<DetailClientDTO>>(clients);

    // // Return the mapped result
    // return new PagedList<DetailClientDTO>(result,
    //                                       count,
    //                                       pageNumber,
    //                                       pageSize);
    return await Task.FromResult<PagedList<DetailClientDTO>>(new PagedList<DetailClientDTO>([],0,new(0,0)));
  }

  // static IClientSpecification BuildSpecification(IClientSpecification baseSpec,
  //                                                Expression<Func<Client, bool>>? filterExpr)
  // {
  //   var spec = baseSpec.AddInclude(c => c.COD)
  //                      .AddInclude(c => c.TfPlan!)
  //                      .AddInclude(c => c.SPRVlans);

  //   if (filterExpr is not null)
  //     spec.AddFilter(filterExpr);

  //   return (IClientSpecification)spec;
  // }
}

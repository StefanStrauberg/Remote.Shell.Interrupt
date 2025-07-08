namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetClientsByFilter;

public record GetClientsByFilterQuery(RequestParameters RequestParameters) 
  : IQuery<PagedList<ShortClientDTO>>;

internal class GetClientsByFilterQueryHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                              IClientSpecification specification,
                                              IQueryFilterParser queryFilterParser,
                                              IMapper mapper)
  : IQueryHandler<GetClientsByFilterQuery, PagedList<ShortClientDTO>>
{
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

  // static IClientSpecification BuildSpecification(IClientSpecification baseSpec,
  //                                                Expression<Func<Client, bool>>? filterExpr)
  // {
  //   var spec = baseSpec;

  //   if (filterExpr is not null)
  //       spec.AddFilter(filterExpr);

  //   return spec;
  // }
}

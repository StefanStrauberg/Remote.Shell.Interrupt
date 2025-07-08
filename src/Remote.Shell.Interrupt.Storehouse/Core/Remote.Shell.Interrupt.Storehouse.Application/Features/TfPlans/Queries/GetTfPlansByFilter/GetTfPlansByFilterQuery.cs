namespace Remote.Shell.Interrupt.Storehouse.Application.Features.TfPlans.Queries.GetTfPlansByFilter;

/// <summary>
/// Defines a query for retrieving a filtered and paginated list of <see cref="TfPlanDTO"/> items.
/// </summary>
/// <param name="Parameters">Contains filtering and pagination parameters.</param>
public sealed record GetTfPlansByFilterQuery(RequestParameters Parameters)
    : FindEntitiesByFilterQuery<TfPlanDTO>(Parameters);

/// <summary>
/// Handles <see cref="GetTfPlansByFilterQuery"/> by applying filter logic,
/// pagination configuration, and entity-to-DTO transformation using base functionality.
/// </summary>
/// <param name="locBillUnitOfWork">Provides access to <see cref="TfPlan"/> repositories.</param>
/// <param name="specification">Base specification used to layer filtering conditions.</param>
/// <param name="queryFilterParser">Parses filter strings into query expressions.</param>
/// <param name="mapper">Maps domain entities to their corresponding DTOs.</param>
internal class GetTfPlansByFilterQueryHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                              ITfPlanSpecification specification,
                                              IQueryFilterParser queryFilterParser,
                                              IMapper mapper)
    : FindEntitiesByFilterQueryHandler<TfPlan, TfPlanDTO, GetTfPlansByFilterQuery>(specification, queryFilterParser, mapper)
{
  /// <summary>
  /// Retrieves all <see cref="TfPlan"/> entities matching the specified filter and pagination context.
  /// </summary>
  /// <param name="spec">The filtering specification.</param>
  /// <param name="cancellationToken">Propagates cancellation signals.</param>
  /// <returns>A collection of <see cref="TfPlan"/> domain entities.</returns>
  protected override async Task<IEnumerable<TfPlan>> FetchEntitiesAsync(ISpecification<TfPlan> spec,
                                                                        CancellationToken cancellationToken)
    => await locBillUnitOfWork.TfPlans.GetManyShortAsync(spec, cancellationToken);

  /// <summary>
  /// Retrieves the total count of <see cref="TfPlan"/> entities matching the given filter specification.
  /// </summary>
  /// <param name="spec">Specification used for filtering and counting entities.</param>
  /// <param name="cancellationToken">Token used to cancel the operation if needed.</param>
  /// <returns>The count of matching <see cref="TfPlan"/> records.</returns>
  protected override async Task<int> CountResultsAsync(ISpecification<TfPlan> spec,
                                                       CancellationToken cancellationToken)
    => await locBillUnitOfWork.TfPlans.GetCountAsync(spec, cancellationToken);
}

namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Workflows.Queries.GetWorkflowsByFilter;

/// <summary>
/// Defines a query for retrieving a paginated and optionally filtered list of <see cref="WorkflowSummaryDTO"/> items.
/// </summary>
public sealed record GetWorkflowsByFilterQuery(RequestParameters Parameters)
  : FindEntitiesByFilterQuery<WorkflowSummaryDTO>(Parameters);

/// <summary>
/// Handles <see cref="GetWorkflowsByFilterQuery"/>. Includes Nodes/Edges so
/// <see cref="WorkflowSummaryDTO.NodeCount"/>/<see cref="WorkflowSummaryDTO.EdgeCount"/> can be computed.
/// </summary>
internal class GetWorkflowsByFilterQueryHandler(IWorkflowUnitOfWork workflowUnitOfWork,
                                                IWorkflowSpecification specification,
                                                IQueryFilterParser queryFilterParser,
                                                IMapper mapper)
  : FindEntitiesByFilterQueryHandler<WorkflowDefinition, WorkflowSummaryDTO, GetWorkflowsByFilterQuery>(specification, queryFilterParser, mapper)
{
  protected override ISpecification<WorkflowDefinition> BuildSpecification(RequestParameters requestParameters)
  {
    var spec = base.BuildSpecification(requestParameters);

    spec.AddInclude(x => x.Nodes);
    spec.AddInclude(x => x.Edges);

    return spec;
  }

  protected override async Task<IEnumerable<WorkflowDefinition>> FetchEntitiesAsync(ISpecification<WorkflowDefinition> specification,
                                                                                    CancellationToken cancellationToken)
    => await workflowUnitOfWork.Workflows.GetManyWithChildrenAsync(specification, cancellationToken);

  protected override async Task<int> CountResultsAsync(ISpecification<WorkflowDefinition> specification,
                                                       CancellationToken cancellationToken)
    => await workflowUnitOfWork.Workflows.GetCountAsync(specification, cancellationToken);
}

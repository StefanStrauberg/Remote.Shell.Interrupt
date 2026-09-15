namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Workflows.Queries.GetWorkflowById;

/// <summary>
/// Represents a query to retrieve a single <see cref="WorkflowDTO"/> (full graph) by its unique identifier.
/// </summary>
public sealed record GetWorkflowByIdQuery(Guid Id)
  : FindEntityByIdQuery<WorkflowDTO>(Id);

/// <summary>
/// Handles <see cref="GetWorkflowByIdQuery"/>, including the workflow's Nodes and Edges.
/// </summary>
internal class GetWorkflowByIdQueryHandler(IWorkflowUnitOfWork workflowUnitOfWork,
                                           IWorkflowSpecification specification,
                                           IQueryFilterParser queryFilterParser,
                                           IMapper mapper)
  : FindEntityByIdQueryHandler<WorkflowDefinition, WorkflowDTO, GetWorkflowByIdQuery>(specification, queryFilterParser, mapper)
{
  protected override ISpecification<WorkflowDefinition> BuildSpecification(Guid entityId)
  {
    var spec = base.BuildSpecification(entityId);

    spec.AddInclude(x => x.Nodes);
    spec.AddInclude(x => x.Edges);

    return spec;
  }

  protected override async Task EnsureEntityExistAsync(ISpecification<WorkflowDefinition> specification,
                                                       CancellationToken cancellationToken)
  {
    bool exists = await workflowUnitOfWork.Workflows.AnyByQueryAsync(specification, cancellationToken);

    if (exists is not true)
      throw new EntityNotFoundException(typeof(WorkflowDefinition), specification.Criterias?.ToString() ?? nameof(BaseEntity.Id));
  }

  protected override async Task<WorkflowDefinition> FetchEntityAsync(ISpecification<WorkflowDefinition> specification,
                                                                     CancellationToken cancellationToken)
    => await workflowUnitOfWork.Workflows.GetOneWithChildrenAsync(specification, cancellationToken);
}

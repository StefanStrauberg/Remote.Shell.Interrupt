namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Workflows.Commands.ExecuteWorkflow;

/// <summary>
/// Runs a persisted <see cref="WorkflowDefinition"/> against a live device, identified by
/// Host/Community directly (mirrors <c>SNMPGetCommand</c>) rather than a persisted
/// Gate/NetworkDevice - a workflow is reusable across any device with the same shape.
/// </summary>
public record ExecuteWorkflowCommand(Guid WorkflowId, string Host, string Community)
  : ICommand<WorkflowExecutionResultDTO>;

/// <summary>
/// Handles <see cref="ExecuteWorkflowCommand"/> by loading the graph, running it through the
/// engine, and mapping the result. A workflow-level failure (bad OID, unreachable device, a
/// decision with no matching edge) comes back as <c>Success:false</c> plus the partial
/// <c>Steps</c> trail rather than as a thrown HTTP error - the trail up to the failing node is
/// the whole point of surfacing it.
/// </summary>
internal class ExecuteWorkflowCommandHandler(IWorkflowUnitOfWork workflowUnitOfWork,
                                             IWorkflowSpecification specification,
                                             IQueryFilterParser queryFilterParser,
                                             IWorkflowEngine engine,
                                             IMapper mapper)
  : ICommandHandler<ExecuteWorkflowCommand, WorkflowExecutionResultDTO>
{
  public async Task<WorkflowExecutionResultDTO> Handle(ExecuteWorkflowCommand request, CancellationToken cancellationToken)
  {
    var filterExpr = queryFilterParser.ParseFilters<WorkflowDefinition>(RequestParametersFactory.ForId(request.WorkflowId).Filters);
    var spec = specification.Clone();

    if (filterExpr is not null)
      spec.AddFilter(filterExpr);

    spec.AddInclude(x => x.Nodes);
    spec.AddInclude(x => x.Edges);

    bool exists = await workflowUnitOfWork.Workflows.AnyByQueryAsync(spec, cancellationToken);

    if (exists is not true)
      throw new EntityNotFoundException(typeof(WorkflowDefinition), request.WorkflowId.ToString());

    var workflow = await workflowUnitOfWork.Workflows.GetOneWithChildrenAsync(spec, cancellationToken);

    var context = new WorkflowContext(request.Host, request.Community, workflow);

    var result = await engine.ExecuteAsync(workflow, context, cancellationToken);

    return mapper.Map<WorkflowExecutionResultDTO>(result);
  }
}

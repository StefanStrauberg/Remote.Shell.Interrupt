namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Workflows.Commands.UpdateWorkflow;

/// <summary>
/// Command to replace an existing <see cref="WorkflowDefinition"/>'s graph (name/version/
/// start node plus its full node/edge set) with the contents of an <see cref="UpdateWorkflowDTO"/>.
/// </summary>
public record UpdateWorkflowCommand(UpdateWorkflowDTO WorkflowDto) : ICommand<Unit>;

/// <summary>
/// Handles <see cref="UpdateWorkflowCommand"/> as a full graph replace rather than a scalar
/// field patch: the generic <c>UpdateEntityCommandHandler</c> base maps a DTO onto a tracked
/// entity in one Mapster call, which doesn't give predictable delete/insert semantics for
/// a node/edge collection. Instead this handler fetches the entity <b>tracked</b> (with its
/// current Nodes/Edges included), clears those collections, and repopulates them from the
/// DTO - EF Core's change tracker deletes the orphaned rows and inserts the new ones on
/// <see cref="IUnitOfWork.Complete"/> because the required FK relationship is configured
/// with cascade delete (see WorkflowDefinitionConfiguration).
/// </summary>
internal class UpdateWorkflowCommandHandler(IWorkflowUnitOfWork workflowUnitOfWork,
                                            IWorkflowSpecification specification,
                                            IQueryFilterParser queryFilterParser,
                                            IMapper mapper)
  : ICommandHandler<UpdateWorkflowCommand, Unit>
{
  public async Task<Unit> Handle(UpdateWorkflowCommand request, CancellationToken cancellationToken)
  {
    var dto = request.WorkflowDto;

    var spec = BuildSpecification(dto.Id);

    bool exists = await workflowUnitOfWork.Workflows.AnyByQueryAsync(spec, cancellationToken);

    if (exists is not true)
      throw new EntityNotFoundException(typeof(WorkflowDefinition), dto.Id.ToString());

    var entity = await workflowUnitOfWork.Workflows.GetOneWithChildrenTrackedAsync(spec, cancellationToken);

    if (entity.Status != WorkflowStatus.Draft)
      throw new WorkflowNotEditableException(
        $"Workflow '{entity.Name}' is {entity.Status} and its graph is immutable. Archive it and create a new workflow instead.");

    entity.Name = dto.Name;
    entity.Version = dto.Version;
    entity.StartNodeId = dto.StartNodeId;

    entity.Nodes.Clear();
    entity.Nodes.AddRange(mapper.Map<List<NodeDefinition>>(dto.Nodes));

    entity.Edges.Clear();
    entity.Edges.AddRange(mapper.Map<List<EdgeDefinition>>(dto.Edges));

    workflowUnitOfWork.Complete();

    return Unit.Value;
  }

  ISpecification<WorkflowDefinition> BuildSpecification(Guid entityId)
  {
    var filterExpr = queryFilterParser.ParseFilters<WorkflowDefinition>(RequestParametersFactory.ForId(entityId).Filters);
    var spec = specification.Clone();

    if (filterExpr is not null)
      spec.AddFilter(filterExpr);

    spec.AddInclude(x => x.Nodes);
    spec.AddInclude(x => x.Edges);

    return spec;
  }
}

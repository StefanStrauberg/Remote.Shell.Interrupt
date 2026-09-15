namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Workflows.Commands.ArchiveWorkflow;

/// <summary>
/// Transitions a <see cref="WorkflowDefinition"/> to <see cref="WorkflowStatus.Archived"/>
/// from either <see cref="WorkflowStatus.Draft"/> or <see cref="WorkflowStatus.Published"/>.
/// </summary>
public record ArchiveWorkflowCommand(Guid Id) : ICommand<Unit>;

internal class ArchiveWorkflowCommandHandler(IWorkflowUnitOfWork workflowUnitOfWork,
                                             IWorkflowSpecification specification,
                                             IQueryFilterParser queryFilterParser)
  : ICommandHandler<ArchiveWorkflowCommand, Unit>
{
  public async Task<Unit> Handle(ArchiveWorkflowCommand request, CancellationToken cancellationToken)
  {
    var spec = BuildSpecification(request.Id);

    bool exists = await workflowUnitOfWork.Workflows.AnyByQueryAsync(spec, cancellationToken);

    if (exists is not true)
      throw new EntityNotFoundException(typeof(WorkflowDefinition), request.Id.ToString());

    var entity = await workflowUnitOfWork.Workflows.GetOneShortAsync(spec, cancellationToken);

    if (entity.Status == WorkflowStatus.Archived)
      throw new WorkflowNotEditableException($"Workflow '{entity.Name}' is already Archived.");

    entity.Status = WorkflowStatus.Archived;

    workflowUnitOfWork.Workflows.ReplaceOne(entity);
    workflowUnitOfWork.Complete();

    return Unit.Value;
  }

  ISpecification<WorkflowDefinition> BuildSpecification(Guid entityId)
  {
    var filterExpr = queryFilterParser.ParseFilters<WorkflowDefinition>(RequestParametersFactory.ForId(entityId).Filters);
    var spec = specification.Clone();

    if (filterExpr is not null)
      spec.AddFilter(filterExpr);

    return spec;
  }
}

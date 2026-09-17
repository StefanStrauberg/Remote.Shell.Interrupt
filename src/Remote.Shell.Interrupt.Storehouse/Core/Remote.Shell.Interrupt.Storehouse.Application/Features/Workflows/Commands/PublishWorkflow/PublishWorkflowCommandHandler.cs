using Mediator;

namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Workflows.Commands.PublishWorkflow;

/// <summary>
/// Transitions a <see cref="WorkflowDefinition"/> from <see cref="WorkflowStatus.Draft"/> to
/// <see cref="WorkflowStatus.Published"/>. From that point on its graph is immutable - see
/// UpdateWorkflowCommandHandler.
/// </summary>
public record PublishWorkflowCommand(Guid Id) : CQRS.ICommand<Unit>;

internal class PublishWorkflowCommandHandler(IWorkflowUnitOfWork workflowUnitOfWork,
                                             IWorkflowSpecification specification,
                                             IQueryFilterParser queryFilterParser)
  : CQRS.ICommandHandler<PublishWorkflowCommand, Unit>
{
  public async ValueTask<Unit> Handle(PublishWorkflowCommand request, CancellationToken cancellationToken)
  {
    var spec = BuildSpecification(request.Id);

    bool exists = await workflowUnitOfWork.Workflows.AnyByQueryAsync(spec, cancellationToken);

    if (exists is not true)
      throw new EntityNotFoundException(typeof(WorkflowDefinition), request.Id.ToString());

    var entity = await workflowUnitOfWork.Workflows.GetOneShortAsync(spec, cancellationToken);

    if (entity.Status != WorkflowStatus.Draft)
      throw new WorkflowNotEditableException(
        $"Workflow '{entity.Name}' is {entity.Status}; only a Draft workflow can be published.");

    entity.Status = WorkflowStatus.Published;

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

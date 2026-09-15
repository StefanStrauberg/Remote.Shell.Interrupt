namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Workflows.Commands.DeleteWorkflow;

/// <summary>
/// Command to delete a <see cref="WorkflowDefinition"/> entity by its unique identifier.
/// </summary>
public record DeleteWorkflowCommand(Guid Id) : DeleteEntityCommand(Id);

/// <summary>
/// Handler for processing <see cref="DeleteWorkflowCommand"/> instances. The database-level
/// cascade delete on Nodes/Edges (see WorkflowDefinitionConfiguration) removes the graph's
/// children automatically - no manual cleanup needed here.
/// </summary>
internal class DeleteWorkflowCommandHandler(IWorkflowUnitOfWork workflowUnitOfWork,
                                            IWorkflowSpecification specification,
                                            IQueryFilterParser queryFilterParser)
  : DeleteEntityCommandHandler<WorkflowDefinition, DeleteWorkflowCommand>(specification, queryFilterParser)
{
  protected override async Task EnsureEntityExistAsync(ISpecification<WorkflowDefinition> specification,
                                                       CancellationToken cancellationToken)
  {
    bool exists = await workflowUnitOfWork.Workflows.AnyByQueryAsync(specification, cancellationToken);

    if (exists is not true)
      throw new EntityNotFoundException(typeof(WorkflowDefinition), specification.Criterias?.ToString() ?? nameof(BaseEntity.Id));
  }

  protected override async Task<WorkflowDefinition> FetchEntityAsync(ISpecification<WorkflowDefinition> specification,
                                                                     CancellationToken cancellationToken)
    => await workflowUnitOfWork.Workflows.GetOneShortAsync(specification, cancellationToken);

  protected override void DeleteEntity(WorkflowDefinition entity)
  {
    workflowUnitOfWork.Workflows.DeleteOne(entity);
    workflowUnitOfWork.Complete();
  }
}

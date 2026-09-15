namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.IWorkflowRep;

/// <summary>
/// Defines a repository interface for managing WorkflowDefinition entities, supporting
/// various query and persistence operations.
/// </summary>
public interface IWorkflowDefinitionRepository
  : IManyQueryRepository<WorkflowDefinition>,
    IManyQueryWithRelationsRepository<WorkflowDefinition>,
    IOneQueryRepository<WorkflowDefinition>,
    IOneQueryWithRelationsRepository<WorkflowDefinition>,
    IExistenceQueryRepository<WorkflowDefinition>,
    ICountRepository<WorkflowDefinition>,
    IInsertRepository<WorkflowDefinition>,
    IDeleteRepository<WorkflowDefinition>,
    IReplaceRepository<WorkflowDefinition>
{
  /// <summary>
  /// Retrieves a single WorkflowDefinition together with its Nodes and Edges as a
  /// <b>tracked</b> entity graph, so that clearing/repopulating the child collections
  /// and calling <see cref="Repositories.UnOfWrkRep.IUnitOfWork.Complete"/> is enough for
  /// EF Core to delete the removed children and insert the new ones. Used only by the
  /// full-graph-replace update flow - reads should use <see cref="GetOneWithChildrenAsync"/> instead.
  /// </summary>
  Task<WorkflowDefinition> GetOneWithChildrenTrackedAsync(ISpecification<WorkflowDefinition> specification,
                                                          CancellationToken cancellationToken);
}

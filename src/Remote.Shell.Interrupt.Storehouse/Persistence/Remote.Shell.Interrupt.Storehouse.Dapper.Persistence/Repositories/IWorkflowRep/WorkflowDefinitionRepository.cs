namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.IWorkflowRep;

internal class WorkflowDefinitionRepository(ApplicationDbContext context,
                                            IExistenceQueryRepository<WorkflowDefinition> existenceQueryRepository,
                                            ICountRepository<WorkflowDefinition> countRepository,
                                            IManyQueryRepository<WorkflowDefinition> manyQueryRepository,
                                            IOneQueryRepository<WorkflowDefinition> oneQueryRepository,
                                            IInsertRepository<WorkflowDefinition> insertRepository,
                                            IDeleteRepository<WorkflowDefinition> deleteRepository,
                                            IReplaceRepository<WorkflowDefinition> replaceRepository)
  : IWorkflowDefinitionRepository
{
  async Task<WorkflowDefinition> IOneQueryRepository<WorkflowDefinition>.GetOneShortAsync(ISpecification<WorkflowDefinition> specification,
                                                                                          CancellationToken cancellationToken)
    => await oneQueryRepository.GetOneShortAsync(specification,
                                                 cancellationToken);

  async Task<bool> IExistenceQueryRepository<WorkflowDefinition>.AnyByQueryAsync(ISpecification<WorkflowDefinition> specification,
                                                                                 CancellationToken cancellationToken)
    => await existenceQueryRepository.AnyByQueryAsync(specification,
                                                      cancellationToken);

  async Task<int> ICountRepository<WorkflowDefinition>.GetCountAsync(ISpecification<WorkflowDefinition> specification,
                                                                     CancellationToken cancellationToken)
    => await countRepository.GetCountAsync(specification,
                                           cancellationToken);

  async Task<IEnumerable<WorkflowDefinition>> IManyQueryRepository<WorkflowDefinition>.GetManyShortAsync(ISpecification<WorkflowDefinition> specification,
                                                                                                          CancellationToken cancellationToken)
    => await manyQueryRepository.GetManyShortAsync(specification,
                                                   cancellationToken);

  void IInsertRepository<WorkflowDefinition>.InsertOne(WorkflowDefinition entity)
    => insertRepository.InsertOne(entity);

  void IDeleteRepository<WorkflowDefinition>.DeleteOne(WorkflowDefinition entity)
    => deleteRepository.DeleteOne(entity);

  void IReplaceRepository<WorkflowDefinition>.ReplaceOne(WorkflowDefinition entity)
    => replaceRepository.ReplaceOne(entity);

  async Task<WorkflowDefinition> IOneQueryWithRelationsRepository<WorkflowDefinition>.GetOneWithChildrenAsync(ISpecification<WorkflowDefinition> specification,
                                                                                                              CancellationToken cancellationToken)
    => await context.Set<WorkflowDefinition>()
                    .AsNoTracking()
                    .ApplyIncludes(specification.IncludeChains)
                    .ApplyWhere(specification.Criterias)
                    .FirstAsync(cancellationToken);

  async Task<IEnumerable<WorkflowDefinition>> IManyQueryWithRelationsRepository<WorkflowDefinition>.GetManyWithChildrenAsync(ISpecification<WorkflowDefinition> specification,
                                                                                                                             CancellationToken cancellationToken)
    => await context.Set<WorkflowDefinition>()
                    .AsNoTracking()
                    .ApplyIncludes(specification.IncludeChains)
                    .ApplyWhere(specification.Criterias)
                    .ApplySkip(specification.Skip)
                    .ApplyTake(specification.Take)
                    .ToListAsync(cancellationToken);

  async Task<WorkflowDefinition> IWorkflowDefinitionRepository.GetOneWithChildrenTrackedAsync(ISpecification<WorkflowDefinition> specification,
                                                                                              CancellationToken cancellationToken)
    => await context.Set<WorkflowDefinition>()
                    .ApplyIncludes(specification.IncludeChains)
                    .ApplyWhere(specification.Criterias)
                    .FirstAsync(cancellationToken);
}

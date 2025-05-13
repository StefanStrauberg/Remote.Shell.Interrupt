namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class ExistenceQueryRepository<T>(ApplicationDbContext context)
  : IExistenceQueryRepository<T> where T : BaseEntity
{
  async Task<bool> IExistenceQueryRepository<T>.AnyByQueryAsync(ISpecification<T> specification,
                                                                CancellationToken cancellationToken)
    => await context.Set<T>()
                    .AsNoTracking()
                    .ApplyWhere(specification.Criterias)
                    .AnyAsync(cancellationToken);
}

namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class OneQueryRepository<T>(ApplicationDbContext context)
  : IOneQueryRepository<T> where T : BaseEntity
{
  async Task<T> IOneQueryRepository<T>.GetOneShortAsync(ISpecification<T> specification,
                                                        CancellationToken cancellationToken)
    => await context.Set<T>()
                    .Where(specification.Criterias!)
                    .FirstAsync();
}

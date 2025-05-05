namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class ManyQueryRepository<T>(ApplicationDbContext context)
  : IManyQueryRepository<T> where T : BaseEntity
{
  async Task<IEnumerable<T>> IManyQueryRepository<T>.GetManyShortAsync(ISpecification<T> specification,
                                                                       CancellationToken cancellationToken)
    => await context.Set<T>()
                    .Where(specification.Criterias!)
                    .Skip(specification.Skip)
                    .Take(specification.Take)
                    .ToListAsync();
}

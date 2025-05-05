namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class CountRepository<T>(ApplicationDbContext context)
  : ICountRepository<T> where T : BaseEntity
{
  async Task<int> ICountRepository<T>.GetCountAsync(ISpecification<T> specification,
                                                    CancellationToken cancellationToken)
    => await context.Set<T>()
                    .Select(x => x.Id)
                    .Where(specification.Criterias!)
                    .CountAsync();
}

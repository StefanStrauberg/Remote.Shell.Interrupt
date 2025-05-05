namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class ReadRepository<T>(ApplicationDbContext context)
  : IReadRepository<T> where T : BaseEntity
{
  async Task<IEnumerable<T>> IReadRepository<T>.GetAllAsync(CancellationToken cancellationToken)
    => await context.Set<T>().ToListAsync();
}

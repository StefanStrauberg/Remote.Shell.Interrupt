namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class CountRepository<T>(ApplicationDbContext context)
  : ICountRepository<T> where T : BaseEntity
{
  async Task<int> ICountRepository<T>.GetCountAsync(ISpecification<T> specification,
                                                    CancellationToken cancellationToken)
  {
    var result = await context.Set<T>()
                              .Where(specification.Criterias is null ? x => true : specification.Criterias)
                              .Select(x => x.Id)
                              .ToListAsync(cancellationToken);
    return result.Count;
  }
}

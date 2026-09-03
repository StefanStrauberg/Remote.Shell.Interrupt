namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class CountRepository<T>(ApplicationDbContext context)
  : ICountRepository<T> where T : BaseEntity
{
  async Task<int> ICountRepository<T>.GetCountAsync(ISpecification<T> specification,
                                                    CancellationToken cancellationToken)
  {
    var query = context.Set<T>()
                       .AsNoTracking()
                       .ApplyWhere(specification.Criterias);

    return await query.CountAsync(cancellationToken);
  }
}

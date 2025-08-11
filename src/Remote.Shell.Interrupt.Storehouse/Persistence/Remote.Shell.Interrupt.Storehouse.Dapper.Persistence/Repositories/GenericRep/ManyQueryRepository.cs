namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class ManyQueryRepository<T>(ApplicationDbContext context)
  : IManyQueryRepository<T> where T : BaseEntity
{
  async Task<IEnumerable<T>> IManyQueryRepository<T>.GetManyShortAsync(ISpecification<T> specification,
                                                                       CancellationToken cancellationToken)
    => await context.Set<T>()
                    .AsNoTracking()
                    .ApplyWhere(specification.Criterias)
                    .ApplyOrderBy(specification.OrderBy ?? specification.OrderByDescending, specification.OrderByDescending is not null)
                    .ApplySkip(specification.Skip)
                    .ApplyTake(specification.Take)
                    .ToListAsync(cancellationToken);
}

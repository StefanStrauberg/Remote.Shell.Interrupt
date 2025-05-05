namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class ReplaceRepository<T>(ApplicationDbContext context)
  : IReplaceRepository<T> where T : BaseEntity
{
  void IReplaceRepository<T>.ReplaceOne(T entity)
    => context.Set<T>().Update(entity);
}

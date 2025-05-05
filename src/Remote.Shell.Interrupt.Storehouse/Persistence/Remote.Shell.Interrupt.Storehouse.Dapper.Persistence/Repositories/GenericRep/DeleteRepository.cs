namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class DeleteRepository<T>(ApplicationDbContext context)
  : IDeleteRepository<T> where T : BaseEntity
{
  void IDeleteRepository<T>.DeleteOne(T entity)
    => context.Set<T>().Delete(entity);
}

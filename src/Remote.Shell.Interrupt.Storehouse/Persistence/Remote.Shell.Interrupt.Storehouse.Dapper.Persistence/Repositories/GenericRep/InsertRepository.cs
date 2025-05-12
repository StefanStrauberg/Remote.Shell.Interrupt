namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class InsertRepository<T>(ApplicationDbContext context)
  : IInsertRepository<T> where T : BaseEntity
{
  void IInsertRepository<T>.InsertOne(T entity)
    => context.Set<T>().Add(entity);
}
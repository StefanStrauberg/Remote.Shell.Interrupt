namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.GenericRep;

internal class DeleteRepository<T>(PostgreSQLDapperContext context)
  : IDeleteRepository<T> where T : BaseEntity
{
  void IDeleteRepository<T>.DeleteOne(T entity)
  {
    context.BeginTransaction();
    var baseQuery = $"DELETE FROM \"{GetTableName.Handle<T>()}\" WHERE \"{nameof(BaseEntity.Id)}\"=@Id";
    var connection = context.CreateConnection();
    connection.Execute(baseQuery, new { Id = entity.Id });
  }
}

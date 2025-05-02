namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories.RemBillRep;

internal class RemoteCODRepository(MySQLDapperContext context,
                                   IAppLogger<RemoteCODRepository> logger) 
  : IRemoteCODRepository
{
  async Task<IEnumerable<RemoteCOD>> IRemoteGenericRepository<RemoteCOD>.GetAllAsync(CancellationToken cancellationToken)
  {
    var query = $"SELECT " +
                "c.ID_cod AS \"IdCOD\"," +
                "c.name_cod AS \"NameCOD\", " +
                "c.telephone AS \"Telephone\", " +
                "c.e_mail AS \"Email1\", " +
                "c.e_mail2 AS \"Email2\", " +
                "c.contact AS \"Contact\", " +
                "c.description AS \"Description\", " +
                "c.region AS \"Region\" " +
                "FROM `_cods` as c";

    logger.LogInformation(query);
    
    var connection = await context.CreateConnectionAsync(cancellationToken);

    return await connection.QueryAsync<RemoteCOD>(query);
  }
}

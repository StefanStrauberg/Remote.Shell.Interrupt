
namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class TfPlanRepository(PostgreSQLDapperContext postgreSQLDapperContext) 
    : GenericRepository<TfPlan>(postgreSQLDapperContext), ITfPlanRepository
{
    async Task<IEnumerable<TfPlan>> ITfPlanRepository.GetAllAsync(RequestParameters requestParameters,
                                                                  CancellationToken cancellationToken)
    {
        string columns = GetColumnsAsProperties();
        var offset = (requestParameters.PageNumber - 1) * requestParameters.PageSize;
        var query = $"SELECT {columns} " +
                    $"FROM \"{GetTableName<TfPlan>()}\" " +
                    $"LIMIT {requestParameters.PageSize} OFFSET {offset}";;
        var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
        var tfPlans = await connection.QueryAsync<TfPlan>(query);
        return tfPlans;
    }
}
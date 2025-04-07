namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class TfPlanRepository(PostgreSQLDapperContext postgreSQLDapperContext) 
    : GenericRepository<TfPlan>(postgreSQLDapperContext), ITfPlanRepository
{
    async Task<IEnumerable<TfPlan>> ITfPlanRepository.GetAllTfPlansAsync(RequestParameters requestParameters,
                                                                  CancellationToken cancellationToken)
    {
        string columns = GetColumnsAsProperties();
        var offset = (requestParameters.PageNumber - 1) * requestParameters.PageSize;
        StringBuilder sb = new();
        sb.Append($"SELECT {columns} ");
        sb.Append($"FROM \"{GetTableName<TfPlan>()}\" ");
        sb.Append($"LIMIT {requestParameters.PageSize} OFFSET {offset}");
        
        var query = sb.ToString();
        var connection = await _postgreSQLDapperContext.CreateConnectionAsync(cancellationToken);
        return await connection.QueryAsync<TfPlan>(query);
    }
}
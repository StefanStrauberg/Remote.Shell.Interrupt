namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class TfPlanRepository(PostgreSQLDapperContext postgreSQLDapperContext) : GenericRepository<TfPlan>(postgreSQLDapperContext), ITfPlanRepository
{
}
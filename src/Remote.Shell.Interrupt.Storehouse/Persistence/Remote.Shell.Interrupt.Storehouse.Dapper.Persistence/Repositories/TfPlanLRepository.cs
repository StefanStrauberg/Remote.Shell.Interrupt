namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class TfPlanLRepository(PostgreSQLDapperContext postgreSQLDapperContext) : GenericRepository<TfPlanL>(postgreSQLDapperContext), ITfPlanLRepository
{
}
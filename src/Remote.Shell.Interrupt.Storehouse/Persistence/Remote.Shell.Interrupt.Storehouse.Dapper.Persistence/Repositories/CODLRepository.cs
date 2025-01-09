namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class CODLRepository(PostgreSQLDapperContext postgreSQLDapperContext) : GenericRepository<CODL>(postgreSQLDapperContext), ICODLRepository
{
}
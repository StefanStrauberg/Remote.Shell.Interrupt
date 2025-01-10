namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class CODLRepository(PostgreSQLDapperContext context) : GenericRepository<CODL>(context), ICODLRepository
{
}
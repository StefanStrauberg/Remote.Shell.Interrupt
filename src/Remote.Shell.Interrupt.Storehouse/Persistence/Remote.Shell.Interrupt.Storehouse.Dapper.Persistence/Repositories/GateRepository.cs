namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class GateRepository(PostgreSQLDapperContext context) : GenericRepository<Gate>(context), IGateRepository
{
}

namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class CODRepository(PostgreSQLDapperContext context) : GenericRepository<COD>(context), ICODRepository
{
}
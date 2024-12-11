namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class MACEntityRepository(PostgreSQLDapperContext context) : GenericRepository<MACEntity>(context), IMACEntityRepository
{ }
namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class MACEntityRepository(DapperContext context) : GenericRepository<MACEntity>(context), IMACEntityRepository
{ }
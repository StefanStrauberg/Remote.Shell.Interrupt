namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class PortRepository(DapperContext context) : GenericRepository<Port>(context), IPortRepository
{ }
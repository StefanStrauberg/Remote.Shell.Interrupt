namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class TerminatedNetworkEntityRepository(DapperContext context) : GenericRepository<TerminatedNetworkEntity>(context), ITerminatedNetworkEntityRepository
{ }
namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class TerminatedNetworkEntityRepository(PostgreSQLDapperContext context) : GenericRepository<TerminatedNetworkEntity>(context), ITerminatedNetworkEntityRepository
{ }
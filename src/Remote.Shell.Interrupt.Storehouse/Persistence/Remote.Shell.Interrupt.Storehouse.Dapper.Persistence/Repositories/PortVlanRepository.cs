namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class PortVlanRepository(PostgreSQLDapperContext context) : GenericRepository<PortVlan>(context), IPortVlanRepository
{ }

namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class PortVlanRepository(DapperContext context) : GenericRepository<PortVlan>(context), IPortVlanRepository
{ }

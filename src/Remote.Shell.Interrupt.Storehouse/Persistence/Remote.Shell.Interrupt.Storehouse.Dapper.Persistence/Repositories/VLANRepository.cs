namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class VLANRepository(DapperContext context) : GenericRepository<VLAN>(context), IVLANRepository
{ }

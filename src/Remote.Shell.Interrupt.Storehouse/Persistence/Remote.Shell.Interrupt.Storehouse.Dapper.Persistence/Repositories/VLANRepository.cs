namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class VLANRepository(PostgreSQLDapperContext context) 
    : GenericRepository<VLAN>(context), IVLANRepository
{ }

namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class ARPEntityRepository(DapperContext context) : GenericRepository<ARPEntity>(context), IARPEntityRepository
{ }
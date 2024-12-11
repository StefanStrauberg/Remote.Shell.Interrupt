namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class ARPEntityRepository(PostgreSQLDapperContext context) : GenericRepository<ARPEntity>(context), IARPEntityRepository
{ }
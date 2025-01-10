namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class SPRVlanLsRepository(PostgreSQLDapperContext context) : GenericRepository<SPRVlanL>(context), ISPRVlanLsRepository
{

}

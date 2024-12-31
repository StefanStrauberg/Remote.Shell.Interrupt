
namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class OrganizationRepository(PostgreSQLDapperContext postgreSQLDapperContext) : GenericRepository<ClientCodL>(postgreSQLDapperContext), IOrganizationRepository
{

}

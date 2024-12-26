
namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class OrganizationsRepository(PostgreSQLDapperContext postgreSQLDapperContext) : GenericRepository<ClientCod>(postgreSQLDapperContext), IOrganizationsRepository
{

}

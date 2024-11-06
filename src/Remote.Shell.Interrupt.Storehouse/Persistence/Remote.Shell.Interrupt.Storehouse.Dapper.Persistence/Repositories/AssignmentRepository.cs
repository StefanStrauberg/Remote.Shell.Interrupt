namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class AssignmentRepository(DapperContext context)
  : GenericRepository<Assignment>(context), IAssignmentRepository
{

}

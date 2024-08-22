namespace Remote.Shell.Interrupt.Storehouse.Persistence.Repositories;

internal class AssignmentRepository(IMongoDbSettings settings)
  : GenericRepository<Assignment>(settings), IAssignmentRepository
{
}

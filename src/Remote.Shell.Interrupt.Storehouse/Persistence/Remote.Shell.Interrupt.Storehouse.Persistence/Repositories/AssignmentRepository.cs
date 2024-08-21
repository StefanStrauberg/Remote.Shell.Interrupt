namespace Remote.Shell.Interrupt.Storehouse.Persistence.Repositories;

public class AssignmentRepository(IMongoDbSettings settings) : GenericRepository<Assignment>(settings), IAssignmentRepository
{
}

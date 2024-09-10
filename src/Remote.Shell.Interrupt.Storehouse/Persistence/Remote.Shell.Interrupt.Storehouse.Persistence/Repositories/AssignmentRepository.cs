namespace Remote.Shell.Interrupt.Storehouse.Persistence.Repositories;

internal class AssignmentRepository(ApplicationDbContext dbContext)
  : GenericRepository<Assignment>(dbContext), IAssignmentRepository
{
}

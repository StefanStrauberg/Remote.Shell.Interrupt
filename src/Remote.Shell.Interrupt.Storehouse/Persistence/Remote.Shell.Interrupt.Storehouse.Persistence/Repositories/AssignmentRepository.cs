namespace Remote.Shell.Interrupt.Storehouse.Persistence.Repositories;

internal class AssignmentRepository(IDocumentSession session)
  : GenericRepository<Assignment>(session), IAssignmentRepository
{
}

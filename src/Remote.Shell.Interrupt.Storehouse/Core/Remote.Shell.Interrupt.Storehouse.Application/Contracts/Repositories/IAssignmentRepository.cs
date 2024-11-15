namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface IAssignmentRepository : IGenericRepository<Assignment>
{
  Task<bool> AnyWithTheSameNameAsync(string name,
                                     CancellationToken cancellationToken);

  Task<bool> AnyWithTheSameNameAndDifferentIdAsync(Guid id,
                                                   string name,
                                                   CancellationToken cancellationToken);
}

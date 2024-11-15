
namespace Remote.Shell.Interrupt.Storehouse.Persistence.Repositories;

internal class AssignmentRepository(ApplicationDbContext dbContext)
  : GenericRepository<Assignment>(dbContext), IAssignmentRepository
{
  public async Task<bool> AnyWithTheSameNameAndDifferentIdAsync(Guid id,
                                                                string name,
                                                                CancellationToken cancellationToken)
    => await _dbSet.AsNoTracking()
                   .AnyAsync(x => x.Name == name &&
                             x.Id != id,
                             cancellationToken);

  public async Task<bool> AnyWithTheSameNameAsync(string name,
                                                  CancellationToken cancellationToken)
    => await _dbSet.AsNoTracking()
                   .AnyAsync(x => x.Name == name,
                             cancellationToken);
}

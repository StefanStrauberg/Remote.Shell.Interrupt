using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

namespace Remote.Shell.Interrupt.Storehouse.Persistence.Repositories;

internal class BusinessRuleRepository(ApplicationDbContext dbContext)
  : GenericRepository<BusinessRule>(dbContext), IBusinessRuleRepository
{
  public async Task<IEnumerable<BusinessRule>> GetAllWithChildrenAsync(CancellationToken cancellationToken)
    => await _dbSet
            .AsNoTracking()
            .Include(br => br.Children)
            .Include(br => br.Assignment)
            .ToListAsync(cancellationToken);
}

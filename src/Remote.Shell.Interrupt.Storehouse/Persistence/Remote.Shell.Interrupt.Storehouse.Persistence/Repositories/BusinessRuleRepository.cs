namespace Remote.Shell.Interrupt.Storehouse.Persistence.Repositories;

internal class BusinessRuleRepository(ApplicationDbContext dbContext)
  : GenericRepository<BusinessRule>(dbContext), IBusinessRuleRepository
{
  async Task<IEnumerable<BusinessRule>> IBusinessRuleRepository.GetAllWithChildrenAsync(CancellationToken cancellationToken)
    => await _dbSet
            .AsNoTracking()
            .Include(br => br.Children)
            .Include(br => br.Assignment)
            .ToListAsync(cancellationToken);
}

namespace Remote.Shell.Interrupt.Storehouse.Persistence.Repositories;

internal class BusinessRuleRepository(ApplicationDbContext dbContext)
  : GenericRepository<BusinessRule>(dbContext), IBusinessRuleRepository
{
  public async Task<BusinessRule> GetBusinessRulesNodeByIdAsync(Guid id,
                                                             CancellationToken cancellationToken)
    => await _dbSet.AsNoTracking()
                   .Include(br => br.Children)
                   .Include(br => br.Assignment)
                   .FirstAsync(x => x.Id == id,
                               cancellationToken);

  async Task<BusinessRule> IBusinessRuleRepository.GetBusinessRulesTreeAsync(CancellationToken cancellationToken)
    => await _dbSet.AsNoTracking()
                   .Include(br => br.Children)
                   .Include(br => br.Assignment)
                   .FirstAsync(cancellationToken);
}

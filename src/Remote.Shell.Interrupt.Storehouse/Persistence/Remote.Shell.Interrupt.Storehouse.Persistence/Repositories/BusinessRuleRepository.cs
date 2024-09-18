using System.Linq.Expressions;

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

  public override async Task<BusinessRule> FindOneAsync(Expression<Func<BusinessRule, bool>> filterExpression, CancellationToken cancellationToken)
    => await _dbSet.AsNoTracking()
                   .Include(br => br.Children)
                   .Include(br => br.Assignment)
                   .FirstAsync(filterExpression, cancellationToken);
}

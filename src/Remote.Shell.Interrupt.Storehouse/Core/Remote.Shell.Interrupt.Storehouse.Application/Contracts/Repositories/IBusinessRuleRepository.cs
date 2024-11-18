
namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface IBusinessRuleRepository : IGenericRepository<BusinessRule>
{
  Task<BusinessRule> GetBusinessRulesTreeAsync(CancellationToken cancellationToken);
  Task<BusinessRule> GetBusinessRulesNodeByIdAsync(Guid id, CancellationToken cancellationToken);
}

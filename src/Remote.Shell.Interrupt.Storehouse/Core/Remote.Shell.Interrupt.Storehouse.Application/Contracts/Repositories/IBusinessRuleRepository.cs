namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface IBusinessRuleRepository : IGenericRepository<BusinessRule>
{
  Task<IEnumerable<BusinessRule>> GetAllWithChildrenAsync(CancellationToken cancellationToken);
}

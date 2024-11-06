namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Repositories;

internal class BusinessRuleRepository(DapperContext context)
  : GenericRepository<BusinessRule>(context), IBusinessRuleRepository
{
  public Task<IEnumerable<BusinessRule>> GetAllWithChildrenAsync(CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}

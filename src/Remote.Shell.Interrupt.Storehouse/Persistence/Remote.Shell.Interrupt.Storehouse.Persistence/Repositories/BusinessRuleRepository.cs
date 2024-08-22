namespace Remote.Shell.Interrupt.Storehouse.Persistence.Repositories;

internal class BusinessRuleRepository(IMongoDbSettings settings)
  : GenericRepository<BusinessRule>(settings), IBusinessRuleRepository
{
}

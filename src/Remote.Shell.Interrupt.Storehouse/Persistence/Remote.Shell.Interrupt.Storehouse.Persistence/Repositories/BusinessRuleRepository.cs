namespace Remote.Shell.Interrupt.Storehouse.Persistence.Repositories;

public class BusinessRuleRepository(IMongoDbSettings settings) : GenericRepository<BusinessRule>(settings), IBusinessRuleRepository
{
}

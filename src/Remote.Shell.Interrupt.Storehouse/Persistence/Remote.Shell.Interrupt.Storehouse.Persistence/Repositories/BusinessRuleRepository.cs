namespace Remote.Shell.Interrupt.Storehouse.Persistence.Repositories;

internal class BusinessRuleRepository(IDocumentSession session)
  : GenericRepository<BusinessRule>(session), IBusinessRuleRepository
{
}

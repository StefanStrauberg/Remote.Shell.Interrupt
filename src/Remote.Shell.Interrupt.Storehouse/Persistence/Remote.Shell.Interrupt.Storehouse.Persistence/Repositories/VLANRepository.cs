namespace Remote.Shell.Interrupt.Storehouse.Persistence.Repositories;

internal class VLANRepository(ApplicationDbContext dbContext)
  : GenericRepository<VLAN>(dbContext), IVLANRepository
{
}

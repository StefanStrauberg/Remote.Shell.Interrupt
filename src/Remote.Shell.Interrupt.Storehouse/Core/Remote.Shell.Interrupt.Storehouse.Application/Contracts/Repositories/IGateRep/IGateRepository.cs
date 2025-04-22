namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.IGateRep;

public interface IGateRepository 
  : IManyQueryRepository<Gate>, 
    IOneQueryRepository<Gate>,
    IExistenceQueryRepository<Gate>, 
    ICountRepository<Gate>,
    IWriteOneRepository<Gate>
{ }

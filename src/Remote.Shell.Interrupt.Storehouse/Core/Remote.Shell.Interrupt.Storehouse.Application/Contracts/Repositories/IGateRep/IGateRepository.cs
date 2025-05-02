namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.IGateRep;

/// <summary>
/// Defines a repository interface for managing Gate entities, supporting various query and persistence operations.
/// </summary>
public interface IGateRepository 
  : IManyQueryRepository<Gate>, 
    IOneQueryRepository<Gate>,
    IExistenceQueryRepository<Gate>, 
    ICountRepository<Gate>,
    IInsertRepository<Gate>,
    IDeleteRepository<Gate>,
    IReplaceRepository<Gate>
{ }

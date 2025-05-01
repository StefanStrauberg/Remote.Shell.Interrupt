namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.UnOfWrkRep;

public interface IGateUnitOfWork
{
  IGateRepository Gates { get; }

  void Complete();
}

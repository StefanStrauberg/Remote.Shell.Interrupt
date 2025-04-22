namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.UnOfWrkRep;

public interface IGateUnitOfWork
{
  IGateRepository GateRepository { get; }

  void Complete();
}

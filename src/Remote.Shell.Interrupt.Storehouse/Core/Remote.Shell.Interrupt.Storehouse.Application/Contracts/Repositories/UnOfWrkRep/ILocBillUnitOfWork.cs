namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.UnOfWrkRep;

public interface ILocBillUnitOfWork
{
  IClientsRepository Clients { get; }
  ICODRepository CODs { get; }
  ITfPlanRepository TfPlans { get; }
  ISPRVlansRepository SPRVlans { get; }

  void Complete();
}
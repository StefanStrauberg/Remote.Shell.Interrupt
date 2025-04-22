namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.RemBillRep;

public interface IRemoteGenericRepository<T> where T : class
{
  Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);
}

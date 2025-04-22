namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.GenericRep;

public interface IReadRepository<T> where T : BaseEntity
{
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);
}

namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.GenericRep;

public interface IExistenceRepository<T> where T : BaseEntity
{
    Task<bool> AnyAsync(CancellationToken cancellationToken);
}

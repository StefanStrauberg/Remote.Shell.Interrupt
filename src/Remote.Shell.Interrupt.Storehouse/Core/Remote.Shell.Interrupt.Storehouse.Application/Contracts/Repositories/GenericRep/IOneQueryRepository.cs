namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.GenericRep;

public interface IOneQueryRepository<T> where T : BaseEntity
{   
    Task<T> GetOneShortAsync(ISpecification<T> specification,
                             CancellationToken cancellationToken);
}
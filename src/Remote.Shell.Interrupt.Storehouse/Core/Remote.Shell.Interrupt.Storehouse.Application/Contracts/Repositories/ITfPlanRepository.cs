
namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface ITfPlanRepository : IGenericRepository<TfPlan>
{
    Task<IEnumerable<TfPlan>> GetAllAsync(RequestParameters requestParameters,
                                          CancellationToken cancellationToken);
}
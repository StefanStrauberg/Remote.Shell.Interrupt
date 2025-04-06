
namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories;

public interface ITfPlanRepository : IGenericRepository<TfPlan>
{
    Task<IEnumerable<TfPlan>> GetAllTfPlansAsync(RequestParameters requestParameters,
                                                 CancellationToken cancellationToken);
}
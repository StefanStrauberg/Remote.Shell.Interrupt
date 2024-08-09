namespace Remote.Shell.Interrupt.Storehouse.Application.Features.OIDRepositories;

public record GetOIDRepositoriesQuery() : IQuery<IEnumerable<OIDRepository>>;

internal class GetOIDRepositoriesQueryHandler(IOIDRepositoryRepository oidRepositoryRepository)
  : IQueryHandler<GetOIDRepositoriesQuery, IEnumerable<OIDRepository>>
{
  readonly IOIDRepositoryRepository _OIDRepositoryRepository = oidRepositoryRepository
    ?? throw new ArgumentNullException(nameof(oidRepositoryRepository));
  async Task<IEnumerable<OIDRepository>> IRequestHandler<GetOIDRepositoriesQuery, IEnumerable<OIDRepository>>.Handle(GetOIDRepositoriesQuery request,
                                                                                                                     CancellationToken cancellationToken)
  {
    var oidRepositories = await _OIDRepositoryRepository.GetAllAsync(cancellationToken);

    return oidRepositories;
  }
}

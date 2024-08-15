namespace Remote.Shell.Interrupt.Storehouse.Application.Features.OIDRepositories;

public record GetOIDTargetsQuery() : IQuery<IEnumerable<OIDTarget>>;

internal class GetOIDTargetsQueryHandler(IOIDTargetRepository oidTargetRepository)
  : IQueryHandler<GetOIDTargetsQuery, IEnumerable<OIDTarget>>
{
  readonly IOIDTargetRepository _OIDTargetRepository = oidTargetRepository
    ?? throw new ArgumentNullException(nameof(oidTargetRepository));

  async Task<IEnumerable<OIDTarget>> IRequestHandler<GetOIDTargetsQuery, IEnumerable<OIDTarget>>.Handle(GetOIDTargetsQuery request,
                                                                                                        CancellationToken cancellationToken)
  {
    var oidTargets = await _OIDTargetRepository.GetAllAsync(cancellationToken);

    return oidTargets;
  }
}

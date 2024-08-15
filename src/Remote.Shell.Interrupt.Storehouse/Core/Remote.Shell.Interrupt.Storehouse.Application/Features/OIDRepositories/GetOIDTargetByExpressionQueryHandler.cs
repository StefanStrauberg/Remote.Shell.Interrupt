namespace Remote.Shell.Interrupt.Storehouse.Application.Features.OIDRepositories;

public record GetOIDTargetByExpressionQuery(Expression<Func<OIDTarget, bool>> FilterExpression) : IQuery<OIDTarget>;

internal class GetOIDTargetByExpressionQueryHandler(IOIDTargetRepository oidTargetRepository)
  : IQueryHandler<GetOIDTargetByExpressionQuery, OIDTarget>
{
  readonly IOIDTargetRepository _OIDTargetRepository = oidTargetRepository
    ?? throw new ArgumentNullException(nameof(oidTargetRepository));

  async Task<OIDTarget> IRequestHandler<GetOIDTargetByExpressionQuery, OIDTarget>.Handle(GetOIDTargetByExpressionQuery request,
                                                                                         CancellationToken cancellationToken)
  {
    var oidRepository = await _OIDTargetRepository.FindOneAsync(request.FilterExpression, cancellationToken)
      ?? throw new EntityNotFoundException(request.FilterExpression.Name!);

    return oidRepository;
  }
}

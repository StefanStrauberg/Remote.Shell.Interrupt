namespace Remote.Shell.Interrupt.Storehouse.Application.Features.OIDRepositories;

public record GetOIDRepositoryByExpressionQuery(Expression<Func<OIDRepository, bool>> FilterExpression) : IQuery<OIDRepository>;

internal class GetOIDRepositoryByExpressionQueryHandler(IOIDRepositoryRepository oidRepositoryRepository)
  : IQueryHandler<GetOIDRepositoryByExpressionQuery, OIDRepository>
{
  readonly IOIDRepositoryRepository _OIDRepositoryRepository = oidRepositoryRepository
    ?? throw new ArgumentNullException(nameof(oidRepositoryRepository));
  async Task<OIDRepository> IRequestHandler<GetOIDRepositoryByExpressionQuery, OIDRepository>.Handle(GetOIDRepositoryByExpressionQuery request,
                                                                                                     CancellationToken cancellationToken)
  {
    var oidRepository = await _OIDRepositoryRepository.FindOneAsync(request.FilterExpression, cancellationToken)
      ?? throw new EntityNotFoundException(request.FilterExpression.Name!);

    return oidRepository;
  }
}

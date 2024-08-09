namespace Remote.Shell.Interrupt.Storehouse.Application.Features.OIDRepositories;

public record UpdateOIDRepositoryCommand(Guid Id,
                                         string Vendor,
                                         Dictionary<string, string> OIDs) : ICommand;

internal class UpdateOIDRepositoryCommandHandler(IOIDRepositoryRepository oidRepositoryRepository)
  : ICommandHandler<UpdateOIDRepositoryCommand, Unit>
{
  readonly IOIDRepositoryRepository _OIDRepositoryRepository = oidRepositoryRepository
    ?? throw new ArgumentNullException(nameof(oidRepositoryRepository));

  async Task<Unit> IRequestHandler<UpdateOIDRepositoryCommand, Unit>.Handle(UpdateOIDRepositoryCommand request,
                                                                            CancellationToken cancellationToken)
  {
    var existingUpdatingOIDRepository = await _OIDRepositoryRepository.ExistsAsync(x => x.Id == request.Id,
                                                                                   cancellationToken);

    if (!existingUpdatingOIDRepository)
      throw new EntityNotFoundException(request.Id.ToString());

    var updateOIDRepository = request.Adapt<OIDRepository>();

    await _OIDRepositoryRepository.ReplaceOneAsync(x => x.Id == request.Id,
                                                   updateOIDRepository,
                                                   cancellationToken);
    return Unit.Value;
  }
}

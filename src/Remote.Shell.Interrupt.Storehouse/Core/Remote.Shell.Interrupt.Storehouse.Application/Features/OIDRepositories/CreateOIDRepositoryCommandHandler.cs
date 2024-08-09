namespace Remote.Shell.Interrupt.Storehouse.Application.Features.OIDRepositories;

public record CreateOIDRepositoryCommand(string Vendor, Dictionary<string, string> OIDs, bool Replace) : ICommand;

internal class CreateOIDRepositoryCommandHandler(IOIDRepositoryRepository oidRepositoryRepository)
  : ICommandHandler<CreateOIDRepositoryCommand, Unit>
{
  readonly IOIDRepositoryRepository _OIDRepositoryRepository = oidRepositoryRepository
    ?? throw new ArgumentNullException(nameof(oidRepositoryRepository));

  async Task<Unit> IRequestHandler<CreateOIDRepositoryCommand, Unit>.Handle(CreateOIDRepositoryCommand request,
                                                                            CancellationToken cancellationToken)
  {
    var existingOIDRepository = await _OIDRepositoryRepository.ExistsAsync(x => x.Vendor == request.Vendor,
                                                                           cancellationToken);

    if (existingOIDRepository && !request.Replace)
      throw new EntityAlreadyExists(request.Vendor);

    var oidRepository = request.Adapt<OIDRepository>();

    await _OIDRepositoryRepository.InsertOneAsync(oidRepository, cancellationToken);

    return Unit.Value;
  }
}

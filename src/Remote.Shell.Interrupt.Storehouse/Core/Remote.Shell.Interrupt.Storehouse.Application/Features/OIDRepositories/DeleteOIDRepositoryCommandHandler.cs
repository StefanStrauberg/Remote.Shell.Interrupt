namespace Remote.Shell.Interrupt.Storehouse.Application.Features.OIDRepositories;

public record DeleteOIDRepositoryCommand(Guid Id) : ICommand;

internal class DeleteOIDRepositoryCommandHandler(IOIDRepositoryRepository oidRepositoryRepository)
  : ICommandHandler<DeleteOIDRepositoryCommand, Unit>
{
  readonly IOIDRepositoryRepository _OIDRepositoryRepository = oidRepositoryRepository
    ?? throw new ArgumentNullException(nameof(oidRepositoryRepository));

  async Task<Unit> IRequestHandler<DeleteOIDRepositoryCommand, Unit>.Handle(DeleteOIDRepositoryCommand request,
                                                                            CancellationToken cancellationToken)
  {
    var existingOIDRepository = await _OIDRepositoryRepository.ExistsAsync(x => x.Id == request.Id,
                                                                           cancellationToken);

    if (!existingOIDRepository)
      throw new EntityNotFoundException(request.Id.ToString());

    await _OIDRepositoryRepository.DeleteOneAsync(x => x.Id == request.Id,
                                                  cancellationToken);
    return Unit.Value;
  }
}

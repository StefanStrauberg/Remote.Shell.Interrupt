namespace Remote.Shell.Interrupt.Storehouse.Application.Features.OIDRepositories;

public record DeleteOIDTargetCommand(Guid Id) : ICommand;

internal class DeleteOIDTargetCommandHandler(IOIDTargetRepository oidTargetRepository)
  : ICommandHandler<DeleteOIDTargetCommand, Unit>
{
  readonly IOIDTargetRepository _OIDTargetRepository = oidTargetRepository
    ?? throw new ArgumentNullException(nameof(oidTargetRepository));

  async Task<Unit> IRequestHandler<DeleteOIDTargetCommand, Unit>.Handle(DeleteOIDTargetCommand request,
                                                                        CancellationToken cancellationToken)
  {
    var existingOIDTarget = await _OIDTargetRepository.ExistsAsync(x => x.Id == request.Id,
                                                                   cancellationToken);

    if (!existingOIDTarget)
      throw new EntityNotFoundException(request.Id.ToString());

    await _OIDTargetRepository.DeleteOneAsync(x => x.Id == request.Id,
                                              cancellationToken);
    return Unit.Value;
  }
}

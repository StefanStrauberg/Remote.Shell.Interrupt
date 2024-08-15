namespace Remote.Shell.Interrupt.Storehouse.Application.Features.OIDRepositories;

public record UpdateOIDTargetCommand(Guid Id,
                                     string OIDTargetName,
                                     TargetAction TargetAction,
                                     string Target,
                                     string OID) : ICommand;

internal class UpdateOIDTargetCommandHandler(IOIDTargetRepository oidTargetRepository)
  : ICommandHandler<UpdateOIDTargetCommand, Unit>
{
  readonly IOIDTargetRepository _OIDTargetRepository = oidTargetRepository
    ?? throw new ArgumentNullException(nameof(oidTargetRepository));

  async Task<Unit> IRequestHandler<UpdateOIDTargetCommand, Unit>.Handle(UpdateOIDTargetCommand request,
                                                                        CancellationToken cancellationToken)
  {
    var existingUpdatingOIDTarget = await _OIDTargetRepository.ExistsAsync(x => x.Id == request.Id,
                                                                           cancellationToken);

    if (!existingUpdatingOIDTarget)
      throw new EntityNotFoundException(request.Id.ToString());

    var updateOIDRepository = request.Adapt<OIDTarget>();

    await _OIDTargetRepository.ReplaceOneAsync(x => x.Id == request.Id,
                                               updateOIDRepository,
                                               cancellationToken);
    return Unit.Value;
  }
}

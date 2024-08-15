namespace Remote.Shell.Interrupt.Storehouse.Application.Features.OIDRepositories;

public record CreateOIDTargetCommand(string OIDTargetName,
                                     TargetAction TargetAction,
                                     string Target,
                                     string OID) : ICommand;

internal class CreateOIDTargetCommandHandler(IOIDTargetRepository oidTargetRepository)
  : ICommandHandler<CreateOIDTargetCommand, Unit>
{
  readonly IOIDTargetRepository _OIDTargetRepository = oidTargetRepository
    ?? throw new ArgumentNullException(nameof(oidTargetRepository));

  async Task<Unit> IRequestHandler<CreateOIDTargetCommand, Unit>.Handle(CreateOIDTargetCommand request,
                                                                            CancellationToken cancellationToken)
  {
    var existingOIDTarget = await _OIDTargetRepository.ExistsAsync(x => x.Target == request.OIDTargetName,
                                                                   cancellationToken);

    if (existingOIDTarget)
      throw new EntityAlreadyExists(request.OIDTargetName);

    var oidTarget = request.Adapt<OIDTarget>();

    await _OIDTargetRepository.InsertOneAsync(oidTarget, cancellationToken);

    return Unit.Value;
  }
}

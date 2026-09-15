using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Identity;

namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Users.Commands.SetUserActive;

/// <summary>
/// Activates or soft-deactivates an account. A deactivated account fails login
/// even with valid credentials, but its data and history are kept.
/// </summary>
public sealed record SetUserActiveCommand(Guid UserId, bool IsActive) : ICommand;

/// <param name="currentUserService">
/// Guards against an admin deactivating their own account - the one account
/// that could otherwise still be used to undo it.
/// </param>
internal class SetUserActiveCommandHandler(IIdentityService identityService,
                                           ICurrentUserService currentUserService)
  : ICommandHandler<SetUserActiveCommand>
{
  async Task<Unit> IRequestHandler<SetUserActiveCommand, Unit>.Handle(SetUserActiveCommand request,
                                                                      CancellationToken cancellationToken)
  {
    if (currentUserService.UserId == request.UserId)
      throw new BadRequestException("You cannot deactivate your own account.");

    await identityService.SetUserActiveAsync(request.UserId, request.IsActive, cancellationToken);

    return Unit.Value;
  }
}

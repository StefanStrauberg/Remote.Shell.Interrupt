using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Identity;

namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Users.Commands.DeleteUser;

/// <summary>
/// Permanently deletes an account.
/// </summary>
public sealed record DeleteUserCommand(Guid UserId) : ICommand;

/// <param name="currentUserService">Guards against an admin deleting their own account.</param>
internal class DeleteUserCommandHandler(IIdentityService identityService,
                                        ICurrentUserService currentUserService)
  : ICommandHandler<DeleteUserCommand>
{
  async Task<Unit> IRequestHandler<DeleteUserCommand, Unit>.Handle(DeleteUserCommand request,
                                                                   CancellationToken cancellationToken)
  {
    if (currentUserService.UserId == request.UserId)
      throw new BadRequestException("You cannot delete your own account.");

    await identityService.DeleteUserAsync(request.UserId, cancellationToken);

    return Unit.Value;
  }
}

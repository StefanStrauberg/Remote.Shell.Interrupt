using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Identity;

namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Users.Commands.UpdateUserRole;

/// <summary>
/// Replaces every role currently held by a user with a single new one ("Admin" or "User").
/// </summary>
public sealed record UpdateUserRoleCommand(Guid UserId, string Role) : ICommand;

/// <param name="currentUserService">
/// Guards against an admin changing their own role - a self-service demotion could
/// lock the only signed-in admin out of the very screen used to undo it.
/// </param>
internal class UpdateUserRoleCommandHandler(IIdentityService identityService,
                                            ICurrentUserService currentUserService)
  : ICommandHandler<UpdateUserRoleCommand>
{
  async Task<Unit> IRequestHandler<UpdateUserRoleCommand, Unit>.Handle(UpdateUserRoleCommand request,
                                                                       CancellationToken cancellationToken)
  {
    if (currentUserService.UserId == request.UserId)
      throw new BadRequestException("You cannot change your own role.");

    await identityService.UpdateUserRoleAsync(request.UserId, request.Role, cancellationToken);

    return Unit.Value;
  }
}

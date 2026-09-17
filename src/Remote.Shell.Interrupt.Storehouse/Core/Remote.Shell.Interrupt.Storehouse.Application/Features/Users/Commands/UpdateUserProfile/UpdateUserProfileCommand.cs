using Mediator;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Identity;

namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Users.Commands.UpdateUserProfile;

/// <summary>
/// Updates an account's email and display name. Unlike role/activation/deletion,
/// there is no self-protection guard here - editing your own profile is safe.
/// </summary>
public sealed record UpdateUserProfileCommand(Guid UserId, string Email, string? FullName) : CQRS.ICommand;

internal class UpdateUserProfileCommandHandler(IIdentityService identityService)
  : CQRS.ICommandHandler<UpdateUserProfileCommand>
{
  async ValueTask<Unit> IRequestHandler<UpdateUserProfileCommand, Unit>.Handle(UpdateUserProfileCommand request,
                                                                          CancellationToken cancellationToken)
  {
    await identityService.UpdateUserProfileAsync(request.UserId, request.Email, request.FullName, cancellationToken);

    return Unit.Value;
  }
}

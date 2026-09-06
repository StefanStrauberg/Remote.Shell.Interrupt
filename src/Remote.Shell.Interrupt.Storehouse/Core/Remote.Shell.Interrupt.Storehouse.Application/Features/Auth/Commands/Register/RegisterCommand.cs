using MediatR;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.CQRS;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Identity;
using Remote.Shell.Interrupt.Storehouse.Application.Models.Auth;

namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Auth.Commands.Register;

/// <summary>
/// Creates a new user account with one of the supported roles ("Admin" or "User").
/// Exposed to administrators only at the API layer.
/// </summary>
public sealed record RegisterCommand(string Email, string Password, string Role) : ICommand<RegistrationResult>
{
    // Prevents the LoggingBehavior pipeline from serializing the password.
    public override string ToString()
        => $"{nameof(RegisterCommand)} {{ {nameof(Email)} = {Email}, {nameof(Password)} = ***, {nameof(Role)} = {Role} }}";
}

/// <summary>
/// Handles <see cref="RegisterCommand"/> by delegating user creation and
/// role assignment to the identity abstraction.
/// </summary>
internal sealed class RegisterCommandHandler(IIdentityService identityService)
    : ICommandHandler<RegisterCommand, RegistrationResult>
{
    async Task<RegistrationResult> IRequestHandler<RegisterCommand, RegistrationResult>.Handle(
        RegisterCommand request, CancellationToken cancellationToken)
        => await identityService.RegisterAsync(request.Email, request.Password, request.Role, cancellationToken);
}

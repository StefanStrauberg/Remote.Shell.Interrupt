using Remote.Shell.Interrupt.Storehouse.Application.Contracts.CQRS;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Identity;
using Remote.Shell.Interrupt.Storehouse.Application.Models.Auth;
using MediatR;

namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Auth.Commands.Login;

/// <summary>
/// Authenticates a user and returns a signed JWT access token.
/// </summary>
public sealed record LoginCommand(string Email, string Password) : ICommand<AuthenticationResult>
{
    // Prevents the LoggingBehavior pipeline from serializing the password.
    public override string ToString()
        => $"{nameof(LoginCommand)} {{ {nameof(Email)} = {Email}, {nameof(Password)} = *** }}";
}

/// <summary>
/// Handles <see cref="LoginCommand"/> by delegating credential validation
/// and token issuance to the identity abstraction.
/// </summary>
internal sealed class LoginCommandHandler(IIdentityService identityService)
    : ICommandHandler<LoginCommand, AuthenticationResult>
{
    async Task<AuthenticationResult> IRequestHandler<LoginCommand, AuthenticationResult>.Handle(
        LoginCommand request, CancellationToken cancellationToken)
        => await identityService.LoginAsync(request.Email, request.Password, cancellationToken);
}

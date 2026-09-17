using Remote.Shell.Interrupt.Storehouse.Application.Contracts.CQRS;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Identity;
using Remote.Shell.Interrupt.Storehouse.Application.Models.Auth;
using Mediator;

namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Auth.Commands.RefreshToken;

/// <summary>
/// Exchanges a refresh token for a new access token and a new (rotated) refresh token.
/// </summary>
public sealed record RefreshTokenCommand(string RefreshToken) : CQRS.ICommand<AuthenticationResult>
{
    // Prevents the LoggingBehavior pipeline from serializing the raw token.
    public override string ToString()
        => $"{nameof(RefreshTokenCommand)} {{ {nameof(RefreshToken)} = *** }}";
}

/// <summary>
/// Handles <see cref="RefreshTokenCommand"/> by delegating validation and
/// rotation to the identity abstraction.
/// </summary>
internal sealed class RefreshTokenCommandHandler(IIdentityService identityService)
    : CQRS.ICommandHandler<RefreshTokenCommand, AuthenticationResult>
{
    async ValueTask<AuthenticationResult> IRequestHandler<RefreshTokenCommand, AuthenticationResult>.Handle(
        RefreshTokenCommand request, CancellationToken cancellationToken)
        => await identityService.RefreshTokenAsync(request.RefreshToken, cancellationToken);
}

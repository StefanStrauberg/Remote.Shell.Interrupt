using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Identity;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Auth.Commands.Login;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Auth.Commands.Register;
using Remote.Shell.Interrupt.Storehouse.Application.Models.Auth;

namespace Remote.Shell.Interrupt.Storehouse.API.Controllers;

/// <summary>
/// Provides endpoints for authentication: JWT issuance, browser cookie
/// sessions, and administrator-driven user registration.
/// </summary>
public class AuthController(ISender sender, IIdentityService identityService)
    : BaseAPIController(sender)
{
    /// <summary>
    /// Validates credentials and returns a signed JWT access token.
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthenticationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginCommand loginCommand,
                                           CancellationToken cancellationToken)
    {
        var result = await Sender.Send(loginCommand, cancellationToken);

        return result.Success ? Ok(result) : Unauthorized(new { result.Error });
    }

    /// <summary>
    /// Creates a new user account and assigns the requested role.
    /// Administrators only.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(RegistrationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterCommand registerCommand,
                                              CancellationToken cancellationToken)
    {
        var result = await Sender.Send(registerCommand, cancellationToken);

        return result.Success ? Ok(result) : BadRequest(new { result.Error });
    }

    /// <summary>
    /// Validates credentials and establishes a browser authentication cookie.
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CookieLogin([FromBody] CookieLoginRequest cookieLoginRequest,
                                                 CancellationToken cancellationToken)
    {
        var result = await identityService.LoginAsync(cookieLoginRequest.Email,
                                                      cookieLoginRequest.Password,
                                                      cancellationToken);

        if (!result.Success || result.UserId is null)
            return Unauthorized(new { result.Error });

        await identityService.SignInWithCookieAsync(result.UserId.Value,
                                                    cookieLoginRequest.IsPersistent,
                                                    cancellationToken);

        return Ok(new { result.UserId, result.Email, result.Roles });
    }

    /// <summary>
    /// Terminates the current browser authentication cookie session.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CookieLogout(CancellationToken cancellationToken)
    {
        await identityService.SignOutCookieAsync(cancellationToken);

        return Ok();
    }
}

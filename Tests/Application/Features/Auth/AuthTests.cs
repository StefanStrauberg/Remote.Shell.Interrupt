using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Remote.Shell.Interrupt.Storehouse.API.Controllers;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Identity;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Auth.Commands.Login;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Auth.Commands.RefreshToken;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Auth.Commands.Register;
using Remote.Shell.Interrupt.Storehouse.Application.Models.Auth;
using Remote.Shell.Interrupt.Storehouse.Application.Validations.Auth;

namespace Tests.Application.Features.Auth;

public class LoginCommandHandlerTests
{
    readonly IIdentityService _identityService = Substitute.For<IIdentityService>();

    [Fact]
    public async Task Handle_DelegatesToIdentityServiceLoginAsync()
    {
        var expected = new AuthenticationResult { Success = true, Token = "jwt" };
        _identityService.LoginAsync("a@test.com", "pw", Arg.Any<CancellationToken>()).Returns(expected);
        var handler = new LoginCommandHandler(_identityService);

        var result = await ((IRequestHandler<LoginCommand, AuthenticationResult>)handler)
            .Handle(new LoginCommand("a@test.com", "pw"), CancellationToken.None);

        result.Should().BeSameAs(expected);
    }

    [Fact]
    public void ToString_NeverIncludesPassword()
    {
        var command = new LoginCommand("a@test.com", "super-secret");

        command.ToString().Should().NotContain("super-secret");
        command.ToString().Should().Contain("a@test.com");
    }
}

public class RegisterCommandHandlerTests
{
    readonly IIdentityService _identityService = Substitute.For<IIdentityService>();

    [Fact]
    public async Task Handle_DelegatesToIdentityServiceRegisterAsync()
    {
        var expected = new RegistrationResult { Success = true, UserId = Guid.NewGuid() };
        _identityService.RegisterAsync("a@test.com", "pw", "Admin", Arg.Any<CancellationToken>()).Returns(expected);
        var handler = new RegisterCommandHandler(_identityService);

        var result = await ((IRequestHandler<RegisterCommand, RegistrationResult>)handler)
            .Handle(new RegisterCommand("a@test.com", "pw", "Admin"), CancellationToken.None);

        result.Should().BeSameAs(expected);
    }

    [Fact]
    public void ToString_NeverIncludesPassword()
    {
        var command = new RegisterCommand("a@test.com", "super-secret", "Admin");

        command.ToString().Should().NotContain("super-secret");
        command.ToString().Should().Contain("a@test.com");
        command.ToString().Should().Contain("Admin");
    }
}

public class RefreshTokenCommandHandlerTests
{
    readonly IIdentityService _identityService = Substitute.For<IIdentityService>();

    [Fact]
    public async Task Handle_DelegatesToIdentityServiceRefreshTokenAsync()
    {
        var expected = new AuthenticationResult { Success = true, Token = "jwt", RefreshToken = "new-refresh" };
        _identityService.RefreshTokenAsync("old-refresh", Arg.Any<CancellationToken>()).Returns(expected);
        var handler = new RefreshTokenCommandHandler(_identityService);

        var result = await ((IRequestHandler<RefreshTokenCommand, AuthenticationResult>)handler)
            .Handle(new RefreshTokenCommand("old-refresh"), CancellationToken.None);

        result.Should().BeSameAs(expected);
    }

    [Fact]
    public void ToString_NeverIncludesRawToken()
    {
        var command = new RefreshTokenCommand("super-secret-token");

        command.ToString().Should().NotContain("super-secret-token");
    }
}

public class RefreshTokenCommandValidatorTests
{
    readonly RefreshTokenCommandValidator _validator = new();

    [Fact]
    public void ValidToken_PassesValidation()
    {
        var result = _validator.Validate(new RefreshTokenCommand("some-token"));

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void MissingToken_FailsValidation(string? token)
    {
        var result = _validator.Validate(new RefreshTokenCommand(token!));

        result.IsValid.Should().BeFalse();
    }
}

public class AuthControllerTests
{
    readonly ISender _sender = Substitute.For<ISender>();
    readonly IIdentityService _identityService = Substitute.For<IIdentityService>();
    readonly AuthController _controller;

    public AuthControllerTests()
    {
        _controller = new AuthController(_sender, _identityService)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };
    }

    [Fact]
    public async Task Login_SuccessfulResult_ReturnsOkWithResult()
    {
        var result = new AuthenticationResult { Success = true, Token = "jwt" };
        _sender.Send(Arg.Any<LoginCommand>(), Arg.Any<CancellationToken>()).Returns(result);

        var response = await _controller.Login(new LoginCommand("a@test.com", "pw"), CancellationToken.None);

        var ok = response.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeSameAs(result);
    }

    [Fact]
    public async Task Login_FailedResult_ReturnsUnauthorizedWithError()
    {
        var result = AuthenticationResult.Failed("Invalid credentials.");
        _sender.Send(Arg.Any<LoginCommand>(), Arg.Any<CancellationToken>()).Returns(result);

        var response = await _controller.Login(new LoginCommand("a@test.com", "wrong"), CancellationToken.None);

        response.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Register_SuccessfulResult_ReturnsOkWithResult()
    {
        var result = new RegistrationResult { Success = true, UserId = Guid.NewGuid() };
        _sender.Send(Arg.Any<RegisterCommand>(), Arg.Any<CancellationToken>()).Returns(result);

        var response = await _controller.Register(new RegisterCommand("a@test.com", "pw", "Admin"), CancellationToken.None);

        var ok = response.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeSameAs(result);
    }

    [Fact]
    public async Task Register_FailedResult_ReturnsBadRequestWithError()
    {
        var result = RegistrationResult.Failed("Role 'Admin' does not exist.");
        _sender.Send(Arg.Any<RegisterCommand>(), Arg.Any<CancellationToken>()).Returns(result);

        var response = await _controller.Register(new RegisterCommand("a@test.com", "pw", "Admin"), CancellationToken.None);

        response.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task CookieLogin_InvalidCredentials_ReturnsUnauthorizedWithoutSigningIn()
    {
        _identityService.LoginAsync("a@test.com", "wrong", Arg.Any<CancellationToken>())
                        .Returns(AuthenticationResult.Failed("Invalid credentials."));

        var response = await _controller.CookieLogin(new CookieLoginRequest("a@test.com", "wrong"), CancellationToken.None);

        response.Should().BeOfType<UnauthorizedObjectResult>();
        await _identityService.DidNotReceiveWithAnyArgs()
            .SignInWithCookieAsync(default, default, default);
    }

    [Fact]
    public async Task CookieLogin_ValidCredentials_SignsInAndReturnsOk()
    {
        var userId = Guid.NewGuid();
        _identityService.LoginAsync("a@test.com", "pw", Arg.Any<CancellationToken>())
                        .Returns(new AuthenticationResult { Success = true, UserId = userId, Email = "a@test.com", Roles = ["Admin"] });

        var response = await _controller.CookieLogin(new CookieLoginRequest("a@test.com", "pw", IsPersistent: true), CancellationToken.None);

        response.Should().BeOfType<OkObjectResult>();
        await _identityService.Received().SignInWithCookieAsync(userId, true, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CookieLogout_CallsSignOutAndReturnsOk()
    {
        var response = await _controller.CookieLogout(CancellationToken.None);

        response.Should().BeOfType<OkResult>();
        await _identityService.Received().SignOutCookieAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RefreshToken_SuccessfulResult_ReturnsOkWithResult()
    {
        var result = new AuthenticationResult { Success = true, Token = "jwt", RefreshToken = "new-refresh" };
        _sender.Send(Arg.Any<RefreshTokenCommand>(), Arg.Any<CancellationToken>()).Returns(result);

        var response = await _controller.RefreshToken(new RefreshTokenCommand("old-refresh"), CancellationToken.None);

        var ok = response.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeSameAs(result);
    }

    [Fact]
    public async Task RefreshToken_FailedResult_ReturnsUnauthorizedWithError()
    {
        var result = AuthenticationResult.Failed("Invalid refresh token.");
        _sender.Send(Arg.Any<RefreshTokenCommand>(), Arg.Any<CancellationToken>()).Returns(result);

        var response = await _controller.RefreshToken(new RefreshTokenCommand("bad-refresh"), CancellationToken.None);

        response.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task RevokeToken_CallsIdentityServiceAndReturnsOkRegardlessOfOutcome()
    {
        var response = await _controller.RevokeToken(new RevokeTokenRequest("some-refresh"), CancellationToken.None);

        response.Should().BeOfType<OkResult>();
        await _identityService.Received().RevokeRefreshTokenAsync("some-refresh", Arg.Any<CancellationToken>());
    }
}

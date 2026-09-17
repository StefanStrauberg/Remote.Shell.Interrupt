using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Identity;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Users.Commands.DeleteUser;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Users.Commands.SetUserActive;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Users.Commands.UpdateUserProfile;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Users.Commands.UpdateUserRole;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Users.Queries.GetUsersByFilter;
using Remote.Shell.Interrupt.Storehouse.Application.DTOs.Users;
using Remote.Shell.Interrupt.Storehouse.Application.Validations.Users;

namespace Tests.Application.Features.Users;

public class GetUsersByFilterQueryHandlerTests
{
    readonly IIdentityService _identityService = Substitute.For<IIdentityService>();

    [Fact]
    public async Task Handle_DelegatesToIdentityService()
    {
        var expected = PagedList<UserDTO>.Empty();
        _identityService.GetUsersByFilterAsync(Arg.Any<RequestParameters>(), Arg.Any<CancellationToken>())
                        .Returns(expected);
        var handler = new GetUsersByFilterQueryHandler(_identityService);
        var parameters = new RequestParameters();

        var result = await handler.Handle(new GetUsersByFilterQuery(parameters), CancellationToken.None);

        result.Should().BeSameAs(expected);
        await _identityService.Received().GetUsersByFilterAsync(parameters, Arg.Any<CancellationToken>());
    }
}

public class UpdateUserRoleCommandHandlerTests
{
    readonly IIdentityService _identityService = Substitute.For<IIdentityService>();
    readonly ICurrentUserService _currentUserService = Substitute.For<ICurrentUserService>();

    [Fact]
    public async Task Handle_TargetingSelf_ThrowsBadRequestWithoutCallingIdentityService()
    {
        var userId = Guid.NewGuid();
        _currentUserService.UserId.Returns(userId);
        var handler = new UpdateUserRoleCommandHandler(_identityService, _currentUserService);

        var act = async () => await ((IRequestHandler<UpdateUserRoleCommand, Unit>)handler)
            .Handle(new UpdateUserRoleCommand(userId, "Admin"), CancellationToken.None);

        await act.Should().ThrowAsync<BadRequestException>();
        await _identityService.DidNotReceiveWithAnyArgs()
            .UpdateUserRoleAsync(default, default!, default);
    }

    [Fact]
    public async Task Handle_TargetingAnotherUser_DelegatesToIdentityService()
    {
        _currentUserService.UserId.Returns(Guid.NewGuid());
        var targetId = Guid.NewGuid();
        var handler = new UpdateUserRoleCommandHandler(_identityService, _currentUserService);

        await ((IRequestHandler<UpdateUserRoleCommand, Unit>)handler)
            .Handle(new UpdateUserRoleCommand(targetId, "Admin"), CancellationToken.None);

        await _identityService.Received().UpdateUserRoleAsync(targetId, "Admin", Arg.Any<CancellationToken>());
    }
}

public class SetUserActiveCommandHandlerTests
{
    readonly IIdentityService _identityService = Substitute.For<IIdentityService>();
    readonly ICurrentUserService _currentUserService = Substitute.For<ICurrentUserService>();

    [Fact]
    public async Task Handle_TargetingSelf_ThrowsBadRequestWithoutCallingIdentityService()
    {
        var userId = Guid.NewGuid();
        _currentUserService.UserId.Returns(userId);
        var handler = new SetUserActiveCommandHandler(_identityService, _currentUserService);

        var act = async () => await ((IRequestHandler<SetUserActiveCommand, Unit>)handler)
            .Handle(new SetUserActiveCommand(userId, false), CancellationToken.None);

        await act.Should().ThrowAsync<BadRequestException>();
        await _identityService.DidNotReceiveWithAnyArgs()
            .SetUserActiveAsync(default, default, default);
    }

    [Fact]
    public async Task Handle_TargetingAnotherUser_DelegatesToIdentityService()
    {
        _currentUserService.UserId.Returns(Guid.NewGuid());
        var targetId = Guid.NewGuid();
        var handler = new SetUserActiveCommandHandler(_identityService, _currentUserService);

        await ((IRequestHandler<SetUserActiveCommand, Unit>)handler)
            .Handle(new SetUserActiveCommand(targetId, false), CancellationToken.None);

        await _identityService.Received().SetUserActiveAsync(targetId, false, Arg.Any<CancellationToken>());
    }
}

public class DeleteUserCommandHandlerTests
{
    readonly IIdentityService _identityService = Substitute.For<IIdentityService>();
    readonly ICurrentUserService _currentUserService = Substitute.For<ICurrentUserService>();

    [Fact]
    public async Task Handle_TargetingSelf_ThrowsBadRequestWithoutCallingIdentityService()
    {
        var userId = Guid.NewGuid();
        _currentUserService.UserId.Returns(userId);
        var handler = new DeleteUserCommandHandler(_identityService, _currentUserService);

        var act = async () => await ((IRequestHandler<DeleteUserCommand, Unit>)handler)
            .Handle(new DeleteUserCommand(userId), CancellationToken.None);

        await act.Should().ThrowAsync<BadRequestException>();
        await _identityService.DidNotReceiveWithAnyArgs().DeleteUserAsync(default, default);
    }

    [Fact]
    public async Task Handle_TargetingAnotherUser_DelegatesToIdentityService()
    {
        _currentUserService.UserId.Returns(Guid.NewGuid());
        var targetId = Guid.NewGuid();
        var handler = new DeleteUserCommandHandler(_identityService, _currentUserService);

        await ((IRequestHandler<DeleteUserCommand, Unit>)handler)
            .Handle(new DeleteUserCommand(targetId), CancellationToken.None);

        await _identityService.Received().DeleteUserAsync(targetId, Arg.Any<CancellationToken>());
    }
}

public class UpdateUserProfileCommandHandlerTests
{
    readonly IIdentityService _identityService = Substitute.For<IIdentityService>();

    [Fact]
    public async Task Handle_DelegatesToIdentityServiceIncludingForSelf()
    {
        var userId = Guid.NewGuid();
        var handler = new UpdateUserProfileCommandHandler(_identityService);

        await ((IRequestHandler<UpdateUserProfileCommand, Unit>)handler)
            .Handle(new UpdateUserProfileCommand(userId, "new@test.com", "New Name"), CancellationToken.None);

        await _identityService.Received()
            .UpdateUserProfileAsync(userId, "new@test.com", "New Name", Arg.Any<CancellationToken>());
    }
}

public class UpdateUserRoleCommandValidatorTests
{
    readonly UpdateUserRoleCommandValidator _validator = new();

    [Fact]
    public void ValidCommand_PassesValidation()
    {
        var result = _validator.Validate(new UpdateUserRoleCommand(Guid.NewGuid(), "Admin"));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void EmptyUserId_FailsValidation()
    {
        var result = _validator.Validate(new UpdateUserRoleCommand(Guid.Empty, "Admin"));

        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("SuperAdmin")]
    [InlineData("")]
    public void InvalidRole_FailsValidation(string role)
    {
        var result = _validator.Validate(new UpdateUserRoleCommand(Guid.NewGuid(), role));

        result.IsValid.Should().BeFalse();
    }
}

public class UpdateUserProfileCommandValidatorTests
{
    readonly UpdateUserProfileCommandValidator _validator = new();

    [Fact]
    public void ValidCommand_PassesValidation()
    {
        var result = _validator.Validate(new UpdateUserProfileCommand(Guid.NewGuid(), "a@test.com", "Name"));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void NullFullName_StillPassesValidation()
    {
        var result = _validator.Validate(new UpdateUserProfileCommand(Guid.NewGuid(), "a@test.com", null));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void EmptyUserId_FailsValidation()
    {
        var result = _validator.Validate(new UpdateUserProfileCommand(Guid.Empty, "a@test.com", null));

        result.IsValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public void InvalidEmail_FailsValidation(string email)
    {
        var result = _validator.Validate(new UpdateUserProfileCommand(Guid.NewGuid(), email, null));

        result.IsValid.Should().BeFalse();
    }
}

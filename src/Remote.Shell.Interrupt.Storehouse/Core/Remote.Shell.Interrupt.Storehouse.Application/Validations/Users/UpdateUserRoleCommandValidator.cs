using FluentValidation;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Users.Commands.UpdateUserRole;
using Remote.Shell.Interrupt.Storehouse.Application.Validations.Auth;

namespace Remote.Shell.Interrupt.Storehouse.Application.Validations.Users;

public class UpdateUserRoleCommandValidator : AbstractValidator<UpdateUserRoleCommand>
{
    public UpdateUserRoleCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId can't be empty");
        RuleFor(x => x.Role).NotNull().WithMessage("Role can't be null")
                            .NotEmpty().WithMessage("Role can't be empty")
                            .Must(role => RegisterCommandValidator.AllowedRoles.Contains(role))
                            .WithMessage("Role must be one of: Admin, User");
    }
}

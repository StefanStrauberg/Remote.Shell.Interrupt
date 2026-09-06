using FluentValidation;

namespace Remote.Shell.Interrupt.Storehouse.Application.Validations.Auth;

public class RegisterCommandValidator : AbstractValidator<Features.Auth.Commands.Register.RegisterCommand>
{
    public static readonly string[] AllowedRoles = ["Admin", "User"];

    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email).NotNull().WithMessage("Email can't be null")
                             .NotEmpty().WithMessage("Email can't be empty")
                             .EmailAddress().WithMessage("Email must be a valid email address");
        RuleFor(x => x.Password).NotNull().WithMessage("Password can't be null")
                                .NotEmpty().WithMessage("Password can't be empty")
                                .MinimumLength(10).WithMessage("Password must be at least 10 characters long");
        RuleFor(x => x.Role).NotNull().WithMessage("Role can't be null")
                            .NotEmpty().WithMessage("Role can't be empty")
                            .Must(role => AllowedRoles.Contains(role))
                            .WithMessage("Role must be one of: Admin, User");
    }
}

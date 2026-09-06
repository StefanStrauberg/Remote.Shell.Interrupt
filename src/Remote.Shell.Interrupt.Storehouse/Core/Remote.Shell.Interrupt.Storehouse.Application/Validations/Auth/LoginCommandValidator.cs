using FluentValidation;

namespace Remote.Shell.Interrupt.Storehouse.Application.Validations.Auth;

public class LoginCommandValidator : AbstractValidator<Features.Auth.Commands.Login.LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotNull().WithMessage("Email can't be null")
                             .NotEmpty().WithMessage("Email can't be empty")
                             .EmailAddress().WithMessage("Email must be a valid email address");
        RuleFor(x => x.Password).NotNull().WithMessage("Password can't be null")
                                .NotEmpty().WithMessage("Password can't be empty");
    }
}

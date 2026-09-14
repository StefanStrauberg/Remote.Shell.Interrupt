using FluentValidation;

namespace Remote.Shell.Interrupt.Storehouse.Application.Validations.Auth;

public class RefreshTokenCommandValidator : AbstractValidator<Features.Auth.Commands.RefreshToken.RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken).NotNull().WithMessage("RefreshToken can't be null")
                                    .NotEmpty().WithMessage("RefreshToken can't be empty");
    }
}

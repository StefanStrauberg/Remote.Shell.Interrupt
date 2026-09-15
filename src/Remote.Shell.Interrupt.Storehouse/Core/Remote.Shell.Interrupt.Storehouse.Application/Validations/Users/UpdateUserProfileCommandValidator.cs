using FluentValidation;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Users.Commands.UpdateUserProfile;

namespace Remote.Shell.Interrupt.Storehouse.Application.Validations.Users;

public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId can't be empty");
        RuleFor(x => x.Email).NotNull().WithMessage("Email can't be null")
                             .NotEmpty().WithMessage("Email can't be empty")
                             .EmailAddress().WithMessage("Email must be a valid email address");
    }
}

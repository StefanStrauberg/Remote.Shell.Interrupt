namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics.Commands.Update;

public class UpdateBusinessRuleDTOValidator : AbstractValidator<UpdateBusinessRuleDTO>
{
    public UpdateBusinessRuleDTOValidator()
    {
        RuleFor(x => x.Name).NotNull().WithMessage("Name can't be null")
                            .NotEmpty().WithMessage("Name can't be empty");
    }
}

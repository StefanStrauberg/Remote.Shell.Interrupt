namespace Remote.Shell.Interrupt.Storehouse.Application.Validators;

public class CreateBusinessRuleDTOValidator : AbstractValidator<CreateBusinessRuleDTO>
{
  public CreateBusinessRuleDTOValidator()
  {
    RuleFor(x => x.Name).NotNull().WithMessage("Name can't be null")
                        .NotEmpty().WithMessage("Name can't be empty");
  }
}
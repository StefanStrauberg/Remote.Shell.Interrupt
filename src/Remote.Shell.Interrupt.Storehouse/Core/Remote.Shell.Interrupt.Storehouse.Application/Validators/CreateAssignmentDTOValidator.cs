namespace Remote.Shell.Interrupt.Storehouse.Application.Validators;

public class CreateAssignmentDTOValidator : AbstractValidator<CreateAssignmentDTO>
{
  public CreateAssignmentDTOValidator()
  {
    RuleFor(x => x.Name).NotNull().WithMessage("Name can't be null")
                        .NotEmpty().WithMessage("Name can't be empty");

    RuleFor(x => x.TypeOfRequest).NotNull().WithMessage("Invalid TypeOfRequest value")
                                 .NotEqual(default(TypeOfRequest)).WithMessage("TypeOfRequest is required");

    RuleFor(x => x.OID).NotNull().WithMessage("OID can't be null")
                       .NotEmpty().WithMessage("OID can't be empty")
                       .WithMessage("OID is required")
                       .Matches(@"^\d+(\.\d+)+$")
                       .WithMessage("Invalid OID. OID should be mtches x.x.x where x are numbers");
  }
}
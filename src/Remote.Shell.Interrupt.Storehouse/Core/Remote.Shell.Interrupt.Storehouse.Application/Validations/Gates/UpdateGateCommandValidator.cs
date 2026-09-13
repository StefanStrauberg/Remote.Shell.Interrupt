namespace Remote.Shell.Interrupt.Storehouse.Application.Validations.Gates;

public class UpdateGateCommandValidator : BaseGateValidator<UpdateGateCommand, UpdateGateDTO>
{
    public UpdateGateCommandValidator() : base(x => x.UpdateGateDTO)
    {
        RuleFor(x => x.UpdateGateDTO.Id).NotNull()
                                        .WithMessage("{PropertyName} can't be null")
                                        .NotEmpty()
                                        .WithMessage("{PropertyName} is required");

        RuleFor(x => x.UpdateGateDTO.IPAddress).NotNull()
                                 .WithMessage("{PropertyName} can't be null")
                                 .NotEmpty()
                                 .WithMessage("{PropertyName} is required")
                                 .Matches(@"^(25[0-5]|2[0-4][0-9]|[0-1]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[0-1]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[0-1]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[0-1]?[0-9][0-9]?)$")
                                 .WithMessage("{PropertyName} must be a valid IPv4 address");
    }
}

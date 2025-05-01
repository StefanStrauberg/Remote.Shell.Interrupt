namespace Remote.Shell.Interrupt.Storehouse.Application.Validations.SNMP;

public class SNMPWalkCommandValidator : AbstractValidator<SNMPWalkCommand>
{
    public SNMPWalkCommandValidator()
    {
        RuleFor(x => x.Host).NotNull().WithMessage("Host can't be null")
                            .NotEmpty().WithMessage("Host can't be empty")
                            .Matches(@"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$")
                            .WithMessage("Invalid host. Host should be IPv4 address");
        RuleFor(x => x.Community).NotNull().WithMessage("Community can't be null")
                                 .NotEmpty().WithMessage("Community can't be empty")
                                 .WithMessage("Community is required");
        RuleFor(x => x.OID).NotNull().WithMessage("OID can't be null")
                           .NotEmpty().WithMessage("OID can't be empty")
                           .WithMessage("OID is required")
                           .Matches(@"^\d+(\.\d+)+$")
                           .WithMessage("Invalid OID. OID should be mtches x.x.x where x are numbers");
    }
}

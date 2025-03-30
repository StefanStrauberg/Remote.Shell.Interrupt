namespace Remote.Shell.Interrupt.Storehouse.Application.Validations.Gates;

public class BaseGateValidator<T, TDto> : AbstractValidator<T> where TDto : BaseGateDTO
{
    public BaseGateValidator(Func<T, TDto> selector)
    {
        RuleFor(x => selector(x).Name).NotNull()
                                      .WithMessage("{PropertyName} can't be null")
                                      .NotEmpty()
                                      .WithMessage("{PropertyName} is required");
        RuleFor(x => selector(x).IPAddress).NotNull()
                                .WithMessage("{PropertyName} can't be null")
                                .NotEmpty()
                                .WithMessage("{PropertyName} is required")
                                .Matches(@"^(25[0-5]|2[0-4][0-9]|[0-1]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[0-1]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[0-1]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[0-1]?[0-9][0-9]?)$")
                                .WithMessage("{PropertyName} must be a valid IPv4 address");
        RuleFor(x => selector(x).Community).NotNull()
                                .WithMessage("{PropertyName} can't be null")
                                .NotEmpty()
                                .WithMessage("{PropertyName} is required");
        RuleFor(x => selector(x).TypeOfNetworkDevice).NotNull()
                                .WithMessage("{PropertyName} can't be null")
                                .NotEmpty()
                                .WithMessage("{PropertyName} is required");
    }
}

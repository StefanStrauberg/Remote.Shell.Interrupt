namespace Remote.Shell.Interrupt.Storehouse.Application.Validations.Gates;

public class BaseGateValidator<T, TDto> : AbstractValidator<T> where TDto : BaseGateDTO
{
    protected Func<T, TDto> _selector;
    public BaseGateValidator(Func<T, TDto> selector)
    {
        RuleFor(x => selector(x).Name).NotNull()
                                      .WithMessage("{PropertyName} can't be null")
                                      .NotEmpty()
                                      .WithMessage("{PropertyName} is required");
        RuleFor(x => selector(x).Community).NotNull()
                                .WithMessage("{PropertyName} can't be null")
                                .NotEmpty()
                                .WithMessage("{PropertyName} is required");
        RuleFor(x => selector(x).TypeOfNetworkDevice).NotNull()
                                .WithMessage("{PropertyName} can't be null")
                                .NotEmpty()
                                .WithMessage("{PropertyName} is required");

        _selector = selector;
    }
}

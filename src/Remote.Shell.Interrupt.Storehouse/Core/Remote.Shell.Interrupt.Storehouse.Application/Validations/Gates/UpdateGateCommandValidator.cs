using Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Commands.Update;

namespace Remote.Shell.Interrupt.Storehouse.Application.Validations.Gates;

public class UpdateGateCommandValidator : BaseGateValidator<UpdateGateCommand, UpdateGateDTO>
{
    public UpdateGateCommandValidator() : base(x => x.UpdateGateDTO)
    {
        RuleFor(x => x.UpdateGateDTO.Id).NotNull()
                                        .WithMessage("{PropertyName} can't be null")
                                        .NotEmpty()
                                        .WithMessage("{PropertyName} is required");
    }
}

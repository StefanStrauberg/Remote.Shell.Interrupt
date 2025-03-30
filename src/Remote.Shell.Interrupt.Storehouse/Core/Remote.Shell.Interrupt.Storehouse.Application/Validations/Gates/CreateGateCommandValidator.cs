namespace Remote.Shell.Interrupt.Storehouse.Application.Validations.Gates;

public class CreateGateCommandValidator : BaseGateValidator<CreateGateCommand, CreateGateDTO>
{
    public CreateGateCommandValidator() : base(x => x.CreateGateDTO)
    {
    }
}

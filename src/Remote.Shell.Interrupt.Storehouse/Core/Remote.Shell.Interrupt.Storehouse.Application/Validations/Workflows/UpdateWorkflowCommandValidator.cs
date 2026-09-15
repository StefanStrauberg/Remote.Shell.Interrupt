namespace Remote.Shell.Interrupt.Storehouse.Application.Validations.Workflows;

public class UpdateWorkflowCommandValidator : BaseWorkflowValidator<UpdateWorkflowCommand, UpdateWorkflowDTO>
{
  public UpdateWorkflowCommandValidator() : base(x => x.WorkflowDto)
  {
    RuleFor(x => x.WorkflowDto.Id).NotNull()
                                  .WithMessage("{PropertyName} can't be null")
                                  .NotEmpty()
                                  .WithMessage("{PropertyName} is required");
  }
}

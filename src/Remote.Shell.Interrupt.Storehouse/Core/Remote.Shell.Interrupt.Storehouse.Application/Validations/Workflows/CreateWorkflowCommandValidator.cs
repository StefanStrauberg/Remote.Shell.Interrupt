namespace Remote.Shell.Interrupt.Storehouse.Application.Validations.Workflows;

public class CreateWorkflowCommandValidator : BaseWorkflowValidator<CreateWorkflowCommand, CreateWorkflowDTO>
{
  public CreateWorkflowCommandValidator() : base(x => x.WorkflowDto)
  { }
}

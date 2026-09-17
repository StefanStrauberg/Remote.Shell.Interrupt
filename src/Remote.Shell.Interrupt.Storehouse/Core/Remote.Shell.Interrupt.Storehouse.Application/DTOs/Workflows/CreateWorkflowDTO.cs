namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Workflows;

public class CreateWorkflowDTO : BaseWorkflowDTO, IRegister
{
  void IRegister.Register(TypeAdapterConfig config)
    => config.NewConfig<CreateWorkflowDTO, WorkflowDefinition>();
}

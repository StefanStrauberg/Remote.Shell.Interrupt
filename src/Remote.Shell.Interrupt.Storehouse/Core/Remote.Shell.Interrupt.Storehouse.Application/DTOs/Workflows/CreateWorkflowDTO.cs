namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Workflows;

public class CreateWorkflowDTO : BaseWorkflowDTO, IMapWith<WorkflowDefinition>
{
  void IMapWith<WorkflowDefinition>.Mapping(Profile profile)
    => profile.CreateMap<CreateWorkflowDTO, WorkflowDefinition>();
}

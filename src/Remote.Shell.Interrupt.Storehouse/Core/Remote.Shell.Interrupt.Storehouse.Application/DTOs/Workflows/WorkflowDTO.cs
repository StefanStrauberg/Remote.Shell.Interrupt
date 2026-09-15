namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Workflows;

/// <summary>
/// The full graph of a workflow (nodes + edges), returned by GetById.
/// </summary>
public class WorkflowDTO : BaseWorkflowDTO, IMapWith<WorkflowDefinition>
{
  public Guid Id { get; set; }

  public string Status { get; set; } = string.Empty;

  void IMapWith<WorkflowDefinition>.Mapping(Profile profile)
    => profile.CreateMap<WorkflowDefinition, WorkflowDTO>();
}

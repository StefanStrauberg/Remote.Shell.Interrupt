namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Workflows;

/// <summary>
/// The full graph of a workflow (nodes + edges), returned by GetById.
/// </summary>
public class WorkflowDTO : BaseWorkflowDTO, IRegister
{
  public Guid Id { get; set; }

  public string Status { get; set; } = string.Empty;

  void IRegister.Register(TypeAdapterConfig config)
    => config.NewConfig<WorkflowDefinition, WorkflowDTO>();
}

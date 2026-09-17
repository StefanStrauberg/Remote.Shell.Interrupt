namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Workflows;

public class WorkflowExecutionResultDTO : IRegister
{
  public bool Success { get; set; } = true;

  public string? Error { get; set; }

  public List<WorkflowExecutionStepDTO> Steps { get; set; } = [];

  public Dictionary<string, object?> FinalVariables { get; set; } = [];

  void IRegister.Register(TypeAdapterConfig config)
    => config.NewConfig<WorkflowExecutionResult, WorkflowExecutionResultDTO>()
             .Map(dest => dest.FinalVariables,
                  src => src.FinalVariables);
}

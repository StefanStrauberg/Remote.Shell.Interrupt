namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Workflows;

public class WorkflowExecutionResultDTO : IMapWith<WorkflowExecutionResult>
{
  public bool Success { get; set; } = true;

  public string? Error { get; set; }

  public List<WorkflowExecutionStepDTO> Steps { get; set; } = [];

  public Dictionary<string, object?> FinalVariables { get; set; } = [];

  void IMapWith<WorkflowExecutionResult>.Mapping(Profile profile)
    => profile.CreateMap<WorkflowExecutionResult, WorkflowExecutionResultDTO>()
              .ForMember(dest => dest.FinalVariables,
                         opt => opt.MapFrom(src => src.FinalVariables));
}

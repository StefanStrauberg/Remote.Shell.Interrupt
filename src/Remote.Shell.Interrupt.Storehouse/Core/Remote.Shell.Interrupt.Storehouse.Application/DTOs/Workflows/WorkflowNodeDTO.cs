namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Workflows;

/// <summary>
/// A single node of a workflow graph. Used both when reading a workflow back and when
/// submitting one to create/update - the caller assigns <see cref="Id"/> itself so that
/// <see cref="WorkflowEdgeDTO"/> entries in the same payload can reference it.
/// </summary>
public class WorkflowNodeDTO : IMapWith<NodeDefinition>
{
  public Guid Id { get; set; }

  public string Type { get; set; } = string.Empty;

  public string Name { get; set; } = string.Empty;

  public string? Key { get; set; }

  [System.Text.Json.Serialization.JsonConverter(typeof(WorkflowDictionaryJsonConverter))]
  public Dictionary<string, object?> Config { get; set; } = [];

  public double PositionX { get; set; }

  public double PositionY { get; set; }

  void IMapWith<NodeDefinition>.Mapping(Profile profile)
    => profile.CreateMap<NodeDefinition, WorkflowNodeDTO>()
              .ReverseMap();
}

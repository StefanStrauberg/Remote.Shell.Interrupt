namespace Remote.Shell.Interrupt.Storehouse.Domain.Workflow;
public class WorkflowContext(NetworkDevice device, WorkflowDefinition workflow)
{
  private readonly Dictionary<string, object?> _variables = [];

  public NetworkDevice Device { get; } = device;

  public WorkflowDefinition Workflow { get; } = workflow;

  public Guid? CurrentNodeId { get; internal set; }

  public void Set(string name,object? value)
  {
    _variables[name] = value;
  }

  public object? Get(string name)
  {
    _variables.TryGetValue(name, out var value);
    return value;
  }
  
  public T? Get<T>(string name)
  {
    var value = Get(name);

    if (value is null)
      return default;

    if (value is T typedValue)
      return typedValue;

    return (T)Convert.ChangeType(value, typeof(T));
  }

  public bool Contains(string name)
  {
    return _variables.ContainsKey(name);
  }

  public IReadOnlyDictionary<string, object?> Variables =>
    _variables;
}
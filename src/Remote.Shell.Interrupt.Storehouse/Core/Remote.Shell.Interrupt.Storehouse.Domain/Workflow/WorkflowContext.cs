namespace Remote.Shell.Interrupt.Storehouse.Domain.Workflow;
public class WorkflowContext(string host, string community, WorkflowDefinition workflow)
{
  private readonly Dictionary<string, object?> _variables = [];

  public string Host { get; } = host;

  public string Community { get; } = community;

  public WorkflowDefinition Workflow { get; } = workflow;

  public Guid? CurrentNodeId { get; set; }

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

    var targetType = typeof(T);
    var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

    if (underlyingType == typeof(Guid) && value is string guidString)
      return (T)(object)Guid.Parse(guidString);

    if (underlyingType.IsEnum && value is string enumString)
      return (T)Enum.Parse(underlyingType, enumString, true);

    return (T)Convert.ChangeType(value, underlyingType);
  }

  public bool Contains(string name)
  {
    return _variables.ContainsKey(name);
  }

  public IReadOnlyDictionary<string, object?> Variables =>
    _variables;
}
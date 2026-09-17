namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Helpers;

/// <summary>
/// EF Core value conversion for a <c>Dictionary&lt;string, object?&gt;</c> column stored as
/// <c>jsonb</c> (e.g. <see cref="NodeDefinition.Config"/>). <see cref="JsonSerializer"/>
/// deserializes untyped values as boxed <see cref="JsonElement"/>, not plain CLR
/// primitives - <see cref="Normalize"/> converts those back to string/long/double/bool/
/// null/List/Dictionary so callers see the same shapes as when the dictionary was built
/// directly in memory (e.g. in a test, or in the workflow engine's node executors).
/// </summary>
internal static class JsonDictionaryConverter
{
  public static readonly ValueConverter<Dictionary<string, object?>, string> Converter = new(
    v => Serialize(v),
    v => Normalize(Deserialize(v)));

  public static readonly ValueComparer<Dictionary<string, object?>> Comparer = new(
    (a, b) => Serialize(a) == Serialize(b),
    v => Serialize(v).GetHashCode(),
    v => Normalize(Deserialize(Serialize(v))));

  static string Serialize(Dictionary<string, object?>? value)
    => JsonSerializer.Serialize(value ?? [], (JsonSerializerOptions?)null);

  static Dictionary<string, object?> Deserialize(string value)
    => JsonSerializer.Deserialize<Dictionary<string, object?>>(value, (JsonSerializerOptions?)null) ?? [];

  public static Dictionary<string, object?> Normalize(Dictionary<string, object?> config)
  {
    var normalized = new Dictionary<string, object?>(config.Count);

    foreach (var (key, value) in config)
      normalized[key] = NormalizeValue(value);

    return normalized;
  }

  static object? NormalizeValue(object? value)
  {
    if (value is not JsonElement element)
      return value;

    return element.ValueKind switch
    {
      JsonValueKind.String => element.GetString(),
      JsonValueKind.Number => element.TryGetInt64(out var integer) ? integer : element.GetDouble(),
      JsonValueKind.True => true,
      JsonValueKind.False => false,
      JsonValueKind.Null => null,
      JsonValueKind.Array => element.EnumerateArray().Select(e => NormalizeValue(e)).ToList(),
      JsonValueKind.Object => element.EnumerateObject().ToDictionary(p => p.Name, p => NormalizeValue(p.Value)),
      _ => value
    };
  }
}

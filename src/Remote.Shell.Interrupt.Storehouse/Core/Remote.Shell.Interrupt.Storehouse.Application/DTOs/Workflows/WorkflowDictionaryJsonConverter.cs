using System.Text.Json;
using System.Text.Json.Serialization;

namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Workflows;

/// <summary>
/// HTTP workflow dictionaries must use the same CLR values as persisted configs.
/// Default object deserialization produces JsonElement, breaking script validation,
/// numeric configuration and nested execution inputs before persistence can normalize them.
/// </summary>
public sealed class WorkflowDictionaryJsonConverter : JsonConverter<Dictionary<string, object?>>
{
  public override Dictionary<string, object?> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    using var document = JsonDocument.ParseValue(ref reader);
    if (document.RootElement.ValueKind != JsonValueKind.Object)
      throw new JsonException("Workflow configuration must be a JSON object.");
    return document.RootElement.EnumerateObject().ToDictionary(p => p.Name, p => ReadValue(p.Value));
  }

  static object? ReadValue(JsonElement value) => value.ValueKind switch
  {
    JsonValueKind.String => value.GetString(),
    JsonValueKind.Number => value.TryGetInt64(out var integer) ? (object)integer : value.GetDouble(),
    JsonValueKind.True => true,
    JsonValueKind.False => false,
    JsonValueKind.Null => null,
    JsonValueKind.Array => value.EnumerateArray().Select(ReadValue).ToList(),
    JsonValueKind.Object => value.EnumerateObject().ToDictionary(p => p.Name, p => ReadValue(p.Value)),
    _ => throw new JsonException("Unsupported workflow value.")
  };

  public override void Write(Utf8JsonWriter writer, Dictionary<string, object?> value, JsonSerializerOptions options)
    => JsonSerializer.Serialize(writer, value, options);
}

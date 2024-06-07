namespace Remote.Shell.Interrupt.SNMPExecutor.SNMPCommandExecutor.Helpers;

internal static class Helper
{
  internal static JObject ConvertVariablesToJsonObject(IList<Variable> variables)
  {
    JObject result = [];
    JArray array = [];

    foreach (var variable in variables)
    {
      JObject variableObject = new()
      {
        [nameof(variable.Id)] = variable.Id.ToString(),
        [nameof(variable.Data.TypeCode)] = variable.Data.TypeCode.ToString(),
        [nameof(variable.Data)] = variable.Data.ToString(),
      };
      array.Add(variableObject);
    }

    result["Variables"] = array;

    return result;
  }
}

namespace Remote.Shell.Interrupt.Storehouse.Application.Helper;

public static class ConditionEvaluator
{
  public static async Task<bool> EvaluateConditionAsync(string condition, object contextObject)
  {
    if (string.IsNullOrWhiteSpace(condition))
    {
      throw new InvalidOperationException("Condition is not set.");
    }

    // Создание глобальных переменных на основе контекста
    var globals = new ScriptGlobals();
    var properties = contextObject.GetType()
                                  .GetProperties();

    foreach (var property in properties)
    {
      var value = property.GetValue(contextObject);
      globals.GetType()
             .GetProperty(property.Name)?
             .SetValue(globals, value);
    }

    // Выполнение скрипта
    try
    {
      var result = await CSharpScript.EvaluateAsync<bool>(code: condition,
                                                          globals: globals);

      return result;
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException("Error evaluating condition.", ex);
    }
  }
}

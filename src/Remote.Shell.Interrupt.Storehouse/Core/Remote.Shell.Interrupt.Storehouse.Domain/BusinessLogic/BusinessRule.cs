namespace Remote.Shell.Interrupt.Storehouse.Domain.BusinessLogic;

public class BusinessRule : BaseEntity
{
  public string Name { get; set; } = string.Empty; // Get Interfaces for Huawei
  public string? Condition { get; set; } // x.Name == Huawei
  public Guid? ParentId { get; set; }
  public List<Guid> Children { get; set; } = [];
  public Guid? AssignmentId { get; set; } // Represent what to do

  public async Task<bool> EvaluateConditionAsync(object contextObject)
  {
    if (string.IsNullOrWhiteSpace(Condition))
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
      var result = await CSharpScript.EvaluateAsync<bool>(code: Condition,
                                                          globals: globals);

      return result;
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException("Error evaluating condition.", ex);
    }
  }
}

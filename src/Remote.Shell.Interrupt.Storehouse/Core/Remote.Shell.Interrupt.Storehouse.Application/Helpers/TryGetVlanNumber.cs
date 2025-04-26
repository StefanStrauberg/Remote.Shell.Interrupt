namespace Remote.Shell.Interrupt.Storehouse.Application.Helpers;

internal static class TryGetVlanNumber
{
    internal static bool Handle(string interfaceName, out int vlanTag)
    {
      StringBuilder result = new();

      foreach (char c in interfaceName)
      {
        if (char.IsDigit(c))  // Проверяем, является ли символ цифрой
        {
          result.Append(c);  // Добавляем цифру в результат
        }
      }

      if (int.TryParse(result.ToString(), out vlanTag))
        return true;
      else
        return false;
    }
}

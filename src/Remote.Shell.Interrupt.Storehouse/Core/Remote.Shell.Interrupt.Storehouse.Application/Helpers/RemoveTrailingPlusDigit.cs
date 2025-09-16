namespace Remote.Shell.Interrupt.Storehouse.Application.Helpers;

internal static class RemoveTrailingPlusDigit
{
  public static string Handle(string input)
  {
    // Проверяем, содержит ли строка нужный формат в конце
    if (string.IsNullOrEmpty(input))
      return input;

    int index = input.LastIndexOf('+');

    // Проверяем, что после + идет цифра и это действительно конец строки
    if (index != -1 && index + 1 < input.Length && char.IsDigit(input[index + 1]))
      return input[..index]; // Возвращаем строку без + и цифры

    return input; // Если формат не найден, возвращаем исходную строку
  }
}

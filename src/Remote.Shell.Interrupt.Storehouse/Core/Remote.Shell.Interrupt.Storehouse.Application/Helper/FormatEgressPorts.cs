namespace Remote.Shell.Interrupt.Storehouse.Application.Helper;

public static class FormatEgressPorts
{
  private static readonly char[] separator = [',', ' '];

  public static int[] Handle(string input)
  {
    // Если строка пустая или null, возвращаем пустой массив
    if (string.IsNullOrWhiteSpace(input))
      return [];

    // Разделяем строку по запятой и пробелам, убираем лишние пробелы и конвертируем в массив целых чисел
    return input
        .Split(separator, StringSplitOptions.RemoveEmptyEntries)
        .Select(int.Parse)
        .ToArray();
  }
}

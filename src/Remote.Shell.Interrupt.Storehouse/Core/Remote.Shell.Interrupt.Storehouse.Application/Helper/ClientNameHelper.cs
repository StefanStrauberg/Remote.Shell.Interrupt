namespace Remote.Shell.Interrupt.Storehouse.Application.Helper;

internal static class ClientNameHelper
{
  public static string ExtractUniqName(IEnumerable<string> clientNames)
  {
      if (clientNames == null || !clientNames.Any())
          return string.Empty;

      // Разбиваем каждое название на токены, используя пробел как разделитель.
      // При этом знаки препинания остаются частью токенов.
      var splittedNames = clientNames.Select(name => name.Split([' '],
                                                                StringSplitOptions.RemoveEmptyEntries));

      // Находим массив с минимальным количеством слов.
      var shortestWords = splittedNames.OrderBy(arr => arr.Length)
                                       .First();

      // Если в минимальном названии нет ни одного слова, возвращаем пустую строку.
      if (shortestWords.Length == 0)
          return string.Empty;

      // Перебор возможных длины последовательности от максимальной к 1.
      for (int length = shortestWords.Length; length >= 1; length--)
      {
          // Проходим по всем возможным позициям старта последовательности в самом коротком наборе слов.
          for (int start = 0; start <= shortestWords.Length - length; start++)
          {
              // Получаем кандидата в виде последовательности слов:
              var candidateWords = shortestWords.Skip(start).Take(length).ToArray();
              // Объединяем в строку для удобства (слова разделяем пробелом).
              string candidate = string.Join(" ", candidateWords);

              // Проверяем, содержится ли данная последовательность во всех названиях.
              if (splittedNames.All(words => ContainsSequence(words, candidateWords)))
              {
                  // Возвращаем первый найденный самый длинный общий корень.
                  return candidate;
              }
          }
      }

      return string.Empty;
  }

  private static bool ContainsSequence(string[] words, string[] candidateWords)
  {
      for (int i = 0; i <= words.Length - candidateWords.Length; i++)
      {
          bool match = true;
          for (int j = 0; j < candidateWords.Length; j++)
          {
              // Сравнение без учета регистра.
              if (!words[i + j].Equals(candidateWords[j], StringComparison.OrdinalIgnoreCase))
              {
                  match = false;
                  break;
              }
          }
          if (match)
              return true;
      }
      return false;
  }
}

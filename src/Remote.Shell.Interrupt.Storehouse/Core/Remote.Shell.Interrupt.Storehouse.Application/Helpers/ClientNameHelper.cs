namespace Remote.Shell.Interrupt.Storehouse.Application.Helpers;

internal static class ClientNameHelper
{
  public static string ExtractUniqName(List<string> names)
  {
    if (names == null || names.Count == 0)
                return string.Empty;

    // Разбиваем на токены
    var tokenLists = names.Select(name => name.Split(' ',StringSplitOptions.RemoveEmptyEntries)
                                             .ToList())
                         .ToList();
    
    // Находим максимальную длину
    int maxLength = tokenLists.Max(t => t.Count);
    
    // Сравниваем токены по позициям
    var commonTokens = new List<string>();

    // Для каждого индекса (столбца) сравниваем токены во всех строках
    for (int i = 0; i < maxLength; i++)
    {
        string tokenReference = null!;
        bool allEqual = true;

        // Проходим по каждому списку токенов
        foreach (var tokens in tokenLists)
        {
            // Если для данной строки токен отсутствует (т.е. строка короче maxLength),
            // считаем значение токена пустой строкой
            string currentToken = i < tokens.Count ? tokens[i] : "";

            if (tokenReference == null)
                tokenReference = currentToken;
            else if (tokenReference != currentToken)
            {
                allEqual = false;
                break;
            }
        }

        // Если токены на данной позиции совпадают, и они не пустые – добавляем в результат
        if (allEqual && !string.IsNullOrEmpty(tokenReference))
            commonTokens.Add(tokenReference);
    }

    return string.Join(' ', commonTokens);
  }
}

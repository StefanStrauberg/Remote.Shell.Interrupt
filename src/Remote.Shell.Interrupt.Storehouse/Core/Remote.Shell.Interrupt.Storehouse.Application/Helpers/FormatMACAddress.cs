namespace Remote.Shell.Interrupt.Storehouse.Application.Helpers;

public static class FormatMACAddress
{
  public static string Handle(string macAddress)
  {
    // Удаляем все пробелы из строки
    var cleanedMac = macAddress.Replace(" ", "");

    // Вставляем двоеточия между каждыми двумя символами
    var formattedMac = string.Join(":", Enumerable.Range(0, cleanedMac.Length / 2)
                                                .Select(i => cleanedMac.Substring(i * 2, 2)));

    return formattedMac;
  }

  public static string HandleMACTable(string oid)
  {
    // Фиксированная часть OID
    string prefix = "1.3.6.1.2.1.17.4.3.1.2";

    // Проверяем, начинается ли строка с фиксированной части
    if (!oid.StartsWith(prefix))
    {
      throw new ArgumentException("OID должен начинаться с " + prefix);
    }

    // Разделяем строку на части
    var parts = oid.Split('.');

    // Проверяем, достаточно ли частей для формирования MAC-адреса
    if (parts.Length < 8) // 6 частей для MAC-адреса + 2 для OID
    {
      throw new ArgumentException("Недостаточно частей для формирования MAC-адреса.");
    }

    // Извлекаем последние 6 частей, которые представляют собой байты MAC-адреса
    byte[] macBytes = new byte[6];
    for (int i = 0; i < 6; i++)
    {
      if (byte.TryParse(parts[parts.Length - 6 + i], out byte value))
      {
        macBytes[i] = value;
      }
      else
      {
        throw new ArgumentException($"Неверное значение для MAC-адреса: {parts[parts.Length - 6 + i]}");
      }
    }

    // Формируем строку MAC-адреса в формате XX:XX:XX:XX:XX:XX
    string macAddress = string.Join(":", macBytes.Select(b => b.ToString("X2")));

    return macAddress;
  }
}

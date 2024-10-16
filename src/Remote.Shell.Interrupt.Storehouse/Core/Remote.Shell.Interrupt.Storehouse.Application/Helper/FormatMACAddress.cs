namespace Remote.Shell.Interrupt.Storehouse.Application.Helper;

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
}

namespace Remote.Shell.Interrupt.Storehouse.Application.Helpers;

public static class FormatEgressPorts
{
  static readonly char[] separator = [',', ' '];

  internal static int[] HandleJuniperData(string input)
  {
    // Если строка пустая или null, возвращаем пустой массив
    if (string.IsNullOrWhiteSpace(input))
      return [];

    // Разделяем строку по запятой и пробелам, убираем лишние пробелы и конвертируем в массив целых чисел
    return input.Split(separator, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToArray();
  }

  public static int[] HandleHuaweiHexStringOld(string input)
  {
    // Разбиваем строку на отдельные байты
    var hexValues = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

    // Список для хранения активных портов
    List<int> activePorts = [];

    // Переменная для отслеживания номера порта
    int portNumber = 1;

    // Проходим по каждому байту
    foreach (var hexValue in hexValues)
    {
      // Преобразуем каждый байт из Hex в двоичный формат
      byte byteValue = Convert.ToByte(hexValue, 16);
      string binaryString = Convert.ToString(byteValue, 2)
                                   .PadLeft(8, '0'); // Преобразуем в строку с 8 битами

      // Проходим по каждому биту в двоичном представлении
      foreach (char bit in binaryString)
      {
        if (bit == '1')
          activePorts.Add(portNumber - 1); // Порт активен

        portNumber++;
      }
    }

    return [.. activePorts]; // Возвращаем активные порты в виде массива
  }

  public static int[] HandleHuaweiHexStringNew(string hexString)
  {
    // Разбиваем строку на отдельные байты
    var hexValues = hexString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    var activeVlans = new List<int>();

    // Текущий номер VLAN
    int vlanNumber = 1;

    // Проходим по каждому байту
    foreach (var hexValue in hexValues)
    {
      // Преобразуем каждый байт из Hex в двоичный формат
      byte byteValue = Convert.ToByte(hexValue, 16);
      string binaryString = Convert.ToString(byteValue, 2)
                            .PadLeft(8, '0'); // Преобразуем в строку с 8 битами

      // Проходим по каждому биту в двоичном представлении
      foreach (char bit in binaryString)
      {
        if (bit == '1')
          activeVlans.Add(vlanNumber);

        vlanNumber++;
      }
    }

    // Возвращаем активные VLAN-ы в виде массива
    return [.. activeVlans];
  }
}

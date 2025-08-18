namespace Remote.Shell.Interrupt.Storehouse.Application.Helpers;

public static class Converter
{
  public static string ArrayToString(IEnumerable<int> array)
    => string.Join(", ", array);
}

namespace Remote.Shell.Interrupt.Storehouse.Application.Helpers.Extensions;

public static class StringExtensions
{
  public static bool ContainsWholeWord(this string input, string word)
  {
    if (string.IsNullOrWhiteSpace(input) || string.IsNullOrWhiteSpace(word))
      return false;

    var pattern = $@"\b{Regex.Escape(word)}\b";
    return Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
  }
}

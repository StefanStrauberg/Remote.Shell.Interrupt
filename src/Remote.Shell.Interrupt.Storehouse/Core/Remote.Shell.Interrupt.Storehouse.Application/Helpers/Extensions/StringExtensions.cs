namespace Remote.Shell.Interrupt.Storehouse.Application.Helpers.Extensions;

/// <summary>
/// Provides extension methods for string operations.
/// </summary>
public static class StringExtensions
{
  /// <summary>
  /// Determines whether the input string contains the specified word as a whole word.
  /// </summary>
  /// <param name="input">The input string to search within.</param>
  /// <param name="word">The word to search for.</param>
  /// <returns>
  /// <c>true</c> if the input contains the word as a standalone token; otherwise, <c>false</c>.
  /// </returns>
  /// <remarks>
  /// Uses regular expressions with word boundaries (<c>\b</c>) to ensure partial matches (e.g., "cat" in "catalog") are excluded.
  /// Returns <c>false</c> if either <paramref name="input"/> or <paramref name="word"/> is null, empty, or whitespace.
  /// </remarks>
  public static bool ContainsWholeWord(this string input, string word)
  {
    if (string.IsNullOrWhiteSpace(input) || string.IsNullOrWhiteSpace(word))
      return false;

    var pattern = $@"\b{Regex.Escape(word)}\b";
    return Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
  }
}

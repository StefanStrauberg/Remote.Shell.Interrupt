namespace Remote.Shell.Interrupt.Storehouse.Application.Helpers;

/// <summary>
/// Removes a trailing plus sign followed by a single digit from strings.
/// </summary>
internal static class RemoveTrailingPlusDigit
{
  /// <summary>
  /// Strips the last occurrence of a '+' followed by a single digit from the end of a string.
  /// Only removes the pattern when the digit is the final character.
  /// </summary>
  /// <param name="input">The input string.</param>
  /// <returns>The string with the trailing +digit removed, or the original string if no such pattern exists.</returns>
  /// <remarks>
  /// Examples: "text+5" → "text", "a+1+2" → "a+1", "text+X" → "text+X" (non-digit preserved).
  /// </remarks>
  public static string Handle(string input)
  {
    if (string.IsNullOrEmpty(input))
      return input;

    int index = input.LastIndexOf('+');

    if (index != -1 && index + 1 < input.Length && char.IsDigit(input[index + 1]))
      return input[..index];

    return input;
  }
}

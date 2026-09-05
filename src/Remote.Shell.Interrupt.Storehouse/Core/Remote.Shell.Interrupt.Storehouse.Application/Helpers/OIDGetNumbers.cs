namespace Remote.Shell.Interrupt.Storehouse.Application.Helpers;

/// <summary>
/// Extracts numeric components from dot-separated OID strings.
/// </summary>
public static class OIDGetNumbers
{
  /// <summary>
  /// Parses the last numeric component from a dot-separated OID string.
  /// </summary>
  /// <param name="oid">The OID string (e.g., "1.3.6.1.2.1.17.4.3.1.2.100").</param>
  /// <returns>The last numeric component as an integer.</returns>
  internal static int HandleLast(string oid)
    => int.Parse(oid.Split('.').Last());

  /// <summary>
  /// Parses the second-to-last numeric component from a dot-separated OID string.
  /// </summary>
  /// <param name="oid">The OID string.</param>
  /// <returns>The second-to-last numeric component as an integer.</returns>
  /// <exception cref="IndexOutOfRangeException">Thrown when the OID has fewer than 2 segments.</exception>
  internal static int HandleLastButOne(string oid)
    => int.Parse(oid.Split('.')[^2]);
}

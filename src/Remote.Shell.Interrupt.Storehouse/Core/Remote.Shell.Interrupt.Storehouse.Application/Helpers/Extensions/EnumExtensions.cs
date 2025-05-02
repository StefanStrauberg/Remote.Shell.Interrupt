namespace Remote.Shell.Interrupt.Storehouse.Application.Helpers.Extensions;

/// <summary>
/// Provides extension methods for working with enumerations.
/// </summary>
internal static class EnumExtensions
{
  /// <summary>
  /// Retrieves the description attribute value of an enum, if available.
  /// </summary>
  /// <param name="val">The enumeration value.</param>
  /// <returns>The description defined in the <see cref="DescriptionAttribute"/> or the enum name if no description is set.</returns>
  internal static string ToDescriptionString(this Enum val)
  {
    var fi = val.GetType().GetField(val.ToString());

    var attributes = (DescriptionAttribute[])fi!.GetCustomAttributes(typeof(DescriptionAttribute), false);

    return attributes.Length > 0 ? attributes[0].Description : val.ToString();
  }
}

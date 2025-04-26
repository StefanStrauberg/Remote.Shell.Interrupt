namespace Remote.Shell.Interrupt.Storehouse.Application.Helpers;

internal static class EnumExtensions
{
  internal static string ToDescriptionString(this Enum val)
  {
    var fi = val.GetType().GetField(val.ToString());

    var attributes = (DescriptionAttribute[])fi!.GetCustomAttributes(typeof(DescriptionAttribute), false);

    return attributes.Length > 0 ? attributes[0].Description : val.ToString();
  }
}

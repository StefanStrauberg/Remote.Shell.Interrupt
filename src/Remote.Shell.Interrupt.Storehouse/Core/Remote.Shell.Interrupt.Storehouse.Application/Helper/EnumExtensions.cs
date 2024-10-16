namespace Remote.Shell.Interrupt.Storehouse.Application.Helper;

public static class EnumExtensions
{
  public static string ToDescriptionString(this Enum val)
  {
    var fi = val.GetType().GetField(val.ToString());

    var attributes = (DescriptionAttribute[])fi!.GetCustomAttributes(typeof(DescriptionAttribute), false);

    return attributes.Length > 0 ? attributes[0].Description : val.ToString();
  }
}

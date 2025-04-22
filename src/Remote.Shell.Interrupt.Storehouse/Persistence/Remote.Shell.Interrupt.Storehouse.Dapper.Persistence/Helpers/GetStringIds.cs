namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Helpers;

internal static class GetStringIds
{
  public static string Handle(List<Guid> ids)
  {
    var sb = new StringBuilder();

    for (int i = 0; i < ids.Count; i++)
    {
      sb.Append('\'');
      sb.Append(ids[i]);
      sb.Append('\'');

      if (i != ids.Count - 1)
        sb.Append(',');
    }

    return sb.ToString();
  }
}

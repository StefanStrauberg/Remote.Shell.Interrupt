namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Helpers;

internal static class GetStringHosts
{
  public static string Handle(List<string> hosts)
  {
    var sb = new StringBuilder();

    for (int i = 0; i < hosts.Count; i++)
    {
      sb.Append('\'');
      sb.Append(hosts[i]);
      sb.Append('\'');

      if (i != hosts.Count - 1)
        sb.Append(',');
    }

    return sb.ToString();
  }
}

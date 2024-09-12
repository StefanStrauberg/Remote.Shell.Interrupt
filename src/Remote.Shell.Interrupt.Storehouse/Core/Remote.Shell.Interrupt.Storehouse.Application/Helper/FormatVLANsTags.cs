namespace Remote.Shell.Interrupt.Storehouse.Application.Helper;

public static class FormatOIDsToVLANsTags
{
  public static int Handle(string oid)
  {
    return int.Parse(oid.Split('.').Last());
  }
}



namespace Remote.Shell.Interrupt.Storehouse.Application.Helper;

public static class OIDGetLastNumbers
{
  internal static int Handle(string oid)
  {
    return int.Parse(oid.Split('.').Last());
  }
}

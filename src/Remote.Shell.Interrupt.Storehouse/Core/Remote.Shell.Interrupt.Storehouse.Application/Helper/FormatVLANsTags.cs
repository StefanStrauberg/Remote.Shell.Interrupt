namespace Remote.Shell.Interrupt.Storehouse.Application.Helper;

public static class OIDGetNumbers
{
  internal static int HandleLast(string oid)
    => int.Parse(oid.Split('.').Last());

  internal static int HandleLastButOne(string oid)
    => int.Parse(oid.Split('.')[^2]);
}

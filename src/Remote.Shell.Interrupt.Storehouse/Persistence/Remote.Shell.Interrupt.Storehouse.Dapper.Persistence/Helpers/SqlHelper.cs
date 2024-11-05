namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Helpers;

public static class SqlHelper<T>
{
  public static string GetWhereClause(Expression<Func<T, bool>> predicate)
  {
    var linqToSql = predicate.ToString()
                             .Replace("(", "")
                             .Replace(")", "")
                             .Replace("=>", "")
                             .Trim();
    return linqToSql;
  }
}

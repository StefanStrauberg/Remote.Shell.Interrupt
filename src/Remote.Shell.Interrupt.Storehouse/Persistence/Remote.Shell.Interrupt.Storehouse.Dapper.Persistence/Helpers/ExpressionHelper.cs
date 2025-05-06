namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Helpers;

internal static class ExpressionHelper
{
  public static string GetMemberName(Expression expression)
  {
    return expression switch
    {
      MemberExpression m => m.Member.Name,
      UnaryExpression u when u.Operand is MemberExpression m => m.Member.Name,
      MethodCallExpression mc when mc.Method.Name == "get_Item" 
          => throw new NotSupportedException("Indexers are not supported."),
      _ => throw new ArgumentException("Invalid member expression.")
    };
  }

  public static string GetMemberName<T>(Expression<Func<T, object>> expression)
  {
    var body = expression.Body;
    if (body is UnaryExpression unary) 
      body = unary.Operand;
    return GetMemberName(body);
  }
}

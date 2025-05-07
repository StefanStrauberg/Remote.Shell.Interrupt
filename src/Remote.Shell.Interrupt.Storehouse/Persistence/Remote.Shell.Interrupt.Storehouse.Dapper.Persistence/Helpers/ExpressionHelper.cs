namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Helpers;


/// <summary>
/// Provides helper methods for extracting member names from expressions.
/// </summary>
internal static class ExpressionHelper
{
  /// <summary>
  /// Extracts the member name from a given expression.
  /// </summary>
  /// <param name="expression">The expression from which the member name is retrieved.</param>
  /// <returns>The name of the member represented by the expression.</returns>
  /// <exception cref="NotSupportedException">Thrown if the expression represents an indexer.</exception>
  /// <exception cref="ArgumentException">Thrown if the expression is not a valid member expression.</exception>
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

  /// <summary>
  /// Extracts the member name from a strongly typed expression.
  /// </summary>
  /// <typeparam name="T">The type of the entity represented in the expression.</typeparam>
  /// <param name="expression">The expression specifying the member.</param>
  /// <returns>The name of the member represented by the expression.</returns>
  public static string GetMemberName<T>(Expression<Func<T, object>> expression)
  {
    var body = expression.Body;
    if (body is UnaryExpression unary) 
      body = unary.Operand;
    return GetMemberName(body);
  }
}

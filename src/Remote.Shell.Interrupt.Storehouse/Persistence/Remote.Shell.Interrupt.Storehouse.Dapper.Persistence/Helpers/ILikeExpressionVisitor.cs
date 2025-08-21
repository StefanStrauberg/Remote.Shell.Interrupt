namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Helpers;

public class ILikeExpressionVisitor : ExpressionVisitor
{
  static readonly MethodInfo ILikeMethod = typeof(NpgsqlDbFunctionsExtensions).GetMethod(nameof(NpgsqlDbFunctionsExtensions.ILike),
                                                                                         [typeof(DbFunctions),
                                                                                         typeof(string),
                                                                                         typeof(string)])!;

  static readonly MethodInfo RegexIsMatchMethod = typeof(Regex).GetMethod(nameof(Regex.IsMatch),
                                                                          [typeof(string),
                                                                           typeof(string),
                                                                           typeof(RegexOptions)
                                                                          ])!;

  static readonly MethodInfo ConcatMethod = typeof(string).GetMethod(nameof(string.Concat),
                                                                     [typeof(string), typeof(string), typeof(string)])!;
                                                                     
  static readonly MethodInfo ContainsWholeWordMethod = typeof(StringExtensions).GetMethod(nameof(StringExtensions.ContainsWholeWord),
                                                                                          [typeof(string), typeof(string)]
                                                                                         )!;

  protected override Expression VisitMethodCall(MethodCallExpression node)
  {
    if (node.Method == ContainsWholeWordMethod ||
        (node.Method.Name == nameof(StringExtensions.ContainsWholeWord) &&
        node.Method.DeclaringType == typeof(StringExtensions)))
    {
      Expression instance;
      Expression argument;

      if (node.Object != null)
      {
        instance = node.Object;
        argument = node.Arguments[0]; // Первый и единственный аргумент
      }
      else
      {
        instance = node.Arguments[0]; // Первый аргумент - текст
        argument = node.Arguments[1]; // Второй аргумент - слово
      }

      var patternExpression = Expression.Call(ConcatMethod, Expression.Constant("\\m"), argument, Expression.Constant("\\M"));

      return Expression.Call(RegexIsMatchMethod,
                             instance,
                             patternExpression,
                             Expression.Constant(RegexOptions.IgnoreCase | RegexOptions.CultureInvariant));
    }
    
    if (node.Method.Name == nameof(string.Contains) &&
        node.Object?.Type == typeof(string) &&
        node.Arguments.Count == 1 &&
        node.Arguments[0].Type == typeof(string))
    {
      var instance = node.Object;
      var argument = node.Arguments[0];

      var patternExpression = Expression.Call(ConcatMethod, Expression.Constant("%"), argument, Expression.Constant("%"));

      return Expression.Call(ILikeMethod,
                             Expression.Constant(EF.Functions),
                             instance!,
                             patternExpression);
    }

    return base.VisitMethodCall(node);
  }
}

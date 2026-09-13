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
        argument = node.Arguments[0]; // The first and only argument
      }
      else
      {
        instance = node.Arguments[0]; // First argument - the text
        argument = node.Arguments[1]; // Second argument - the word
      }

      var escapedWordArgument = EscapeConstantIfPossible(argument, Regex.Escape);

      var patternExpression = Expression.Call(ConcatMethod, Expression.Constant("\\m"), escapedWordArgument, Expression.Constant("\\M"));

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
      var argument = EscapeConstantIfPossible(node.Arguments[0], EscapeLikePattern);

      var patternExpression = Expression.Call(ConcatMethod, Expression.Constant("%"), argument, Expression.Constant("%"));

      return Expression.Call(ILikeMethod,
                             Expression.Constant(EF.Functions),
                             instance!,
                             patternExpression);
    }

    return base.VisitMethodCall(node);
  }

  /// <summary>
  /// Escapes a filter value known at expression-build time so it is treated as a literal
  /// by the target pattern language (LIKE wildcards or regex metacharacters) instead of
  /// being interpreted as part of the pattern.
  /// </summary>
  static Expression EscapeConstantIfPossible(Expression argument, Func<string, string> escape)
    => argument is ConstantExpression { Value: string value }
         ? Expression.Constant(escape(value), typeof(string))
         : argument;

  /// <summary>
  /// Escapes the characters that are significant to Postgres ILIKE (using the default
  /// backslash escape character): the wildcard characters and the escape character itself.
  /// </summary>
  static string EscapeLikePattern(string value)
    => value.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");
}

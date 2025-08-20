using System.Reflection;

namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Helpers;

public class ILikeExpressionVisitor : ExpressionVisitor
{
  static readonly MethodInfo ILikeMethod = typeof(NpgsqlDbFunctionsExtensions).GetMethod(nameof(NpgsqlDbFunctionsExtensions.ILike),
                                                                                         [typeof(DbFunctions),
                                                                                         typeof(string),
                                                                                         typeof(string)])!;

  protected override Expression VisitMethodCall(MethodCallExpression node)
  {
    // Ищем вызов string.Contains
    if (node.Method.Name == nameof(string.Contains) &&
        node.Object?.Type == typeof(string) &&
        node.Arguments.Count == 1 &&
        node.Arguments[0].Type == typeof(string))
    {
      var instance = node.Object;
      var argument = node.Arguments[0];

      // EF.Functions.ILike(instance, "%" + argument + "%")
      var likePattern = Expression.Call(typeof(string).GetMethod(nameof(string.Concat), [typeof(string), typeof(string), typeof(string)])!,
                                        Expression.Constant("\\m"),
                                        argument,
                                        Expression.Constant("\\M"));

      return Expression.Call(ILikeMethod,
                             Expression.Constant(EF.Functions),
                             instance!,
                             likePattern);
    }

    return base.VisitMethodCall(node);
  }
}

namespace Remote.Shell.Interrupt.BuildingBlocks.Helper;

public class ExpressionToStringConverter<T> : ExpressionVisitor
{
  readonly StringBuilder _stringBuilder;

  public ExpressionToStringConverter()
  {
    _stringBuilder = new StringBuilder();
  }

  public string Convert(Expression<Func<T, bool>> expression)
  {
    Visit(expression);
    return _stringBuilder.ToString();
  }

  protected override Expression VisitBinary(BinaryExpression node)
  {
    _stringBuilder.Append('(');
    Visit(node.Left);
    _stringBuilder.Append($" {GetOperator(node.NodeType)} ");
    Visit(node.Right);
    _stringBuilder.Append(')');
    return node;
  }

  protected override Expression VisitMember(MemberExpression node)
  {
    _stringBuilder.Append(node.Member.Name);
    return node;
  }

  protected override Expression VisitConstant(ConstantExpression node)
  {
    // Append the constant value directly
    if (node.Value is string stringValue)
      _stringBuilder.Append($"\"{stringValue}\""); // Quotes for string values
    else
      _stringBuilder.Append(node.Value);
    return node;
  }

  protected override Expression VisitLambda<T1>(Expression<T1> node)
  {
    Visit(node.Body);
    return node;
  }

  static string GetOperator(ExpressionType type)
    => type switch
    {
      ExpressionType.Equal => "==",
      ExpressionType.NotEqual => "!=",
      ExpressionType.GreaterThan => ">",
      ExpressionType.GreaterThanOrEqual => ">=",
      ExpressionType.LessThan => "<",
      ExpressionType.LessThanOrEqual => "<=",
      ExpressionType.AndAlso => "&&",
      ExpressionType.OrElse => "||",
      _ => throw new NotSupportedException($"Operator '{type}' is not supported")
    };
}

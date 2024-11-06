namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Helpers;

public class ExpressionToStringConverter<T> : ExpressionVisitor
{
  private string result = string.Empty;

  public string Convert(Expression<Func<T, bool>> expression)
  {
    result = string.Empty;
    Visit(expression);
    return result;
  }

  protected override Expression VisitLambda<TLambda>(Expression<TLambda> node) // Use TLambda instead of T
  {
    result += "x => ";
    Visit(node.Body);
    return node;
  }

  protected override Expression VisitMember(MemberExpression node)
  {
    result += node.Member.Name;
    return node;
  }

  protected override Expression VisitConstant(ConstantExpression node)
  {
    if (node.Value is Guid guid)
      result += $"new Guid(\"{guid}\")";
    else
      result += $"\"{node.Value}\"";

    return node;
  }

  protected override Expression VisitBinary(BinaryExpression node)
  {
    Visit(node.Left);
    result += $" {node.NodeType} ";
    Visit(node.Right);
    return node;
  }

  protected override Expression VisitParameter(ParameterExpression node)
  {
    result += node.Name;
    return node;
  }
}

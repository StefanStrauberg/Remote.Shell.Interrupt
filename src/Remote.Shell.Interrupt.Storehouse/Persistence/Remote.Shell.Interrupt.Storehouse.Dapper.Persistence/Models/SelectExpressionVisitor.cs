namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Models;

public class SelectExpressionVisitor : ExpressionVisitor
{
  public string SelectedColumns { get; private set; } = "";

  protected override Expression VisitMember(MemberExpression node)
  {
    if (node.Expression is ParameterExpression)
    {
      var columnName = node.Member.Name;
      SelectedColumns += (SelectedColumns == "" ? "" : ", ") + $"\"{columnName}\"";
    }
    
    return base.VisitMember(node);
  }

  protected override Expression VisitNew(NewExpression node)
  {
    foreach (var arg in node.Arguments)
      Visit(arg);
    
    return node;
  }
}

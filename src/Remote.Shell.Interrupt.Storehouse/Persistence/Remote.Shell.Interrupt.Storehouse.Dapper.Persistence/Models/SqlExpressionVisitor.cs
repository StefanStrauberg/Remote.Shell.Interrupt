namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Models;

public class SqlExpressionVisitor : ExpressionVisitor
{
  readonly Stack<string> _stack = new();
  readonly Dictionary<string, object> _parameters = [];
  int _parameterIndex;
  
  public string WhereClause => _stack.Count > 0 ? _stack.Pop() : "";
  public object Parameters => _parameters;

  protected override Expression VisitBinary(BinaryExpression node)
  {
    Visit(node.Left);
    Visit(node.Right);
    
    var right = _stack.Pop();
    var left = _stack.Pop();
    var op = GetOperator(node.NodeType);
    
    _stack.Push($"({left} {op} {right})");
    return node;
  }

  protected override Expression VisitMember(MemberExpression node)
  {
    if (node.Expression is ParameterExpression)
    {
      _stack.Push($"\"{node.Member.Name}\"");
      return node;
    }
    
    throw new NotSupportedException($"Complex member access '{node}' is not supported");
  }

  protected override Expression VisitConstant(ConstantExpression node)
  {
    var paramName = $"@p{_parameterIndex++}";
    object value = node.Value!;

    if (value == null)
      value = DBNull.Value;
    else if (node.Type.IsEnum)
      value = Convert.ChangeType(value, Enum.GetUnderlyingType(node.Type));
    else if (node.Type == typeof(bool))
      value = Convert.ChangeType(value, typeof(bool));

    _parameters.Add(paramName, value!);
    _stack.Push(paramName);
    return node;
  }

  protected override Expression VisitMethodCall(MethodCallExpression node)
  {
    if (node.Method.Name == "Contains" && node.Method.DeclaringType == typeof(string))
    {
      Visit(node.Object);
      Visit(node.Arguments[0]);
      
      var value = _stack.Pop();
      var column = _stack.Pop();
      _stack.Push($"{column} ILIKE {value}");
      _parameters[_parameters.Keys.Last()] = $"%{_parameters[_parameters.Keys.Last()]}%";
      return node;
    }
    
    throw new NotSupportedException($"Method '{node.Method.Name}' is not supported");
  }

  protected override Expression VisitUnary(UnaryExpression node)
  {
    if (node.NodeType == ExpressionType.Not)
    {
      Visit(node.Operand);
      _stack.Push($"NOT {_stack.Pop()}");
      return node;
    }

    return base.VisitUnary(node);
  }

  static string GetOperator(ExpressionType type)
    => type switch
    {
      ExpressionType.Equal => "=",
      ExpressionType.NotEqual => "<>",
      ExpressionType.GreaterThan => ">",
      ExpressionType.GreaterThanOrEqual => ">=",
      ExpressionType.LessThan => "<",
      ExpressionType.LessThanOrEqual => "<=",
      ExpressionType.AndAlso => "AND",
      ExpressionType.OrElse => "OR",
      _ => throw new NotSupportedException($"Operator {type} is not supported")
    };
}

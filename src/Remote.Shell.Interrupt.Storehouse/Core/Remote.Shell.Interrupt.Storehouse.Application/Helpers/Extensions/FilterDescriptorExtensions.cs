namespace Remote.Shell.Interrupt.Storehouse.Application.Helpers.Extensions;

internal static class FilterDescriptorExtensions
{
  public static Expression<Func<T, bool>> ToExpression<T>(this FilterDescriptor filter)
  {
    var parameter = Expression.Parameter(typeof(T), "x");

    var property = filter.PropertyPath.Split('.')
                                      .Aggregate((Expression)parameter,
                                                 Expression.Property);

    // Обрабатываем оператор Contains отдельно: приводим значение к нижнему регистру заранее
    Expression comparison;

    var value = Expression.Constant(filter.Value);
    comparison = filter.Operator switch
    {
      FilterOperator.Equals => Expression.Equal(property, value),
      FilterOperator.NotEquals => Expression.NotEqual(property, value),
      FilterOperator.GraterThan => Expression.GreaterThan(property, value),
      FilterOperator.LessThan => Expression.LessThan(property, value),
      FilterOperator.Contains => Expression.Call(property, GetContainsMethodInfo(), value),
      _ => throw new NotImplementedException($"Operator {filter.Operator} not supported.")
    };

    return Expression.Lambda<Func<T, bool>>(comparison, parameter);
  }

  static MethodInfo GetContainsMethodInfo()
    => typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!;
}

namespace Remote.Shell.Interrupt.Storehouse.Application.Helpers.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="FilterDescriptor"/> class.
/// </summary>
internal static class FilterDescriptorExtensions
{
  /// <summary>
  /// Converts a <see cref="FilterDescriptor"/> into a LINQ expression for filtering entities.
  /// </summary>
  /// <typeparam name="T">The type of the entity to which the filter applies.</typeparam>
  /// <param name="filter">The filter descriptor to convert.</param>
  /// <returns>
  /// A LINQ expression representing the filter logic defined in the descriptor.
  /// </returns>
  /// <exception cref="NotImplementedException">
  /// Thrown if the filter operator is not supported.
  /// </exception>
  public static Expression<Func<T, bool>> ToExpression<T>(this FilterDescriptor filter)
  {
    var parameter = Expression.Parameter(typeof(T), "x");

    // Resolves nested properties from the property path
    var property = filter.PropertyPath.Split('.')
                                      .Aggregate((Expression)parameter,
                                                 Expression.Property);

    // Initialize a comparison expression based on the filter operator
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

  /// <summary>
  /// Retrieves the <see cref="string.Contains(string)"/> method information.
  /// </summary>
  /// <returns>
  /// The method information for the <see cref="string.Contains(string)"/> method.
  /// </returns>
  static MethodInfo GetContainsMethodInfo()
    => typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!;
}

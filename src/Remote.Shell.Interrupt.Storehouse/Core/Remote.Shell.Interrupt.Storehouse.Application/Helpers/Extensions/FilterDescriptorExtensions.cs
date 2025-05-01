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

    // Get the type of the property
    var propertyType = property.Type;

    // Convert filter.Value to the type of the property
    object? convertedValue;

    try
    {
      if (propertyType == typeof(Guid))
        // Преобразуем строку в Guid
        convertedValue = Guid.Parse(filter.Value);
      else
        // Для остальных типов используем Convert.ChangeType
        convertedValue = Convert.ChangeType(filter.Value, propertyType);
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Не удалось преобразовать значение '{filter.Value}' к типу '{propertyType}'.", ex);
    }

    // Create a constant expression with the converted value
    var value = Expression.Constant(convertedValue, propertyType);

    Expression comparison = filter.Operator switch
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

using System.Collections;

namespace Remote.Shell.Interrupt.Storehouse.QueryFilterParser.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="FilterDescriptor"/> class.
/// </summary>
internal static class FilterDescriptorExtensions
{
  public static Expression<Func<T, bool>> ToExpression<T>(this FilterDescriptor filter)
  {
      var parameter = Expression.Parameter(typeof(T), "x");
      var members = filter.PropertyPath.Split('.');
      Expression body = BuildNestedExpression(parameter, members, 0, filter.Operator, filter.Value);
      
      return Expression.Lambda<Func<T, bool>>(body, parameter);
  }

  private static Expression BuildNestedExpression(Expression expression, string[] members, int index, FilterOperator op, string valueStr)
  {
    // Получаем следующее свойство в цепочке
    Expression property = Expression.PropertyOrField(expression, members[index]);
    var propertyType = property.Type;

    // Если это последний элемент пути – строим сравнение
    if (index == members.Length - 1)
    {
      object convertedValue;
      try
      {
        if (propertyType == typeof(Guid))
          convertedValue = Guid.Parse(valueStr);
        else if (propertyType.IsEnum)
          convertedValue = Enum.Parse(propertyType, valueStr, ignoreCase: true);
        else
          convertedValue = Convert.ChangeType(valueStr, propertyType);
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException($"Не удалось преобразовать значение '{valueStr}' к типу '{propertyType}'.", ex);
      }

      var constant = Expression.Constant(convertedValue, propertyType);
      return BuildComparisonExpression(property, constant, op);
    }
    else
    {
      // Если текущее свойство – коллекция (но не строка),
      // генерируем вызов Any(...) для дальнейшей фильтрации
      if (IsEnumerableButNotString(propertyType))
      {
        var elementType = GetElementType(propertyType) 
          ?? throw new InvalidOperationException($"Не удалось определить элемент коллекции для типа {propertyType}.");

        // Создадим параметр для элементов коллекции
        var lambdaParameter = Expression.Parameter(elementType, "e");
        // Рекурсивно строим выражение для оставшейся части пути
        Expression nested = BuildNestedExpression(lambdaParameter, members, index + 1, op, valueStr);
        var lambda = Expression.Lambda(nested, lambdaParameter);
        
        // Ищем метод Enumerable.Any<T>(IEnumerable<T>, Func<T, bool>)
        MethodInfo anyMethod = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                                                 .First(m => m.Name == "Any" && m.GetParameters().Length == 2)
                                                 .MakeGenericMethod(elementType);

        return Expression.Call(anyMethod, property, lambda);
      }
      else
      {
        // Если не коллекция – продолжаем цепочку навигации
        return BuildNestedExpression(property, members, index + 1, op, valueStr);
      }
    }
  }

  static bool IsEnumerableButNotString(Type type)
    => typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string);

  static Type GetElementType(Type type)
  {
    if (type.IsArray)
      return type.GetElementType()!;
    if (type.IsGenericType)
      return type.GetGenericArguments()[0];
    return null!;
  }

  static Expression BuildComparisonExpression(Expression left, Expression right, FilterOperator op)
    => op switch
    {
        FilterOperator.Equals => Expression.Equal(left, right),
        FilterOperator.NotEquals => Expression.NotEqual(left, right),
        FilterOperator.GraterThan => Expression.GreaterThan(left, right),
        FilterOperator.LessThan => Expression.LessThan(left, right),
        FilterOperator.Contains => Expression.Call(left, GetContainsMethodInfo(), right),
        _ => throw new NotImplementedException($"Operator {op} is not supported.")
    };

  /// <summary>
  /// Retrieves the <see cref="string.Contains(string)"/> method information.
  /// </summary>
  /// <returns>
  /// The method information for the <see cref="string.Contains(string)"/> method.
  /// </returns>
  static MethodInfo GetContainsMethodInfo()
    => typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!;
}

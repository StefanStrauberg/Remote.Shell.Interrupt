namespace Remote.Shell.Interrupt.Storehouse.QueryFilterParser.Extensions;

internal static class FilterDescriptorExtensions
{
  public static Expression<Func<T, bool>> ToExpression<T>(this FilterDescriptor filter)
  {
      var parameter = Expression.Parameter(typeof(T), "x");
      var members = filter.PropertyPath.Split('.');
      Expression body = BuildNestedExpression(parameter, members, 0, filter.Operator, filter.Value);
      
      return Expression.Lambda<Func<T, bool>>(body, parameter);
  }

  static Expression BuildNestedExpression(Expression expression,
                                          string[] members,
                                          int index,
                                          FilterOperator op,
                                          string valueStr)
  {
    // Получаем следующее свойство в цепочке
    Expression property = Expression.PropertyOrField(expression, members[index]);
    var propertyType = property.Type;

    // Если это последний элемент пути – строим сравнение
    if (index == members.Length - 1)
    {
      if (op == FilterOperator.In)
      {
        var rawValues = valueStr.Split(',')
                                .Select(val => ConvertValue(val, propertyType))
                                .ToList();
        var listType = typeof(List<>).MakeGenericType(propertyType);
        var listInstance = (IList)Activator.CreateInstance(listType)!;
        
        foreach (var rawValue in rawValues)
          listInstance.Add(rawValue);

        var constant = Expression.Constant(listInstance, listType);
        return BuildComparisonExpression(property, constant, op);
      }
      else
      {
        object convertedValue = ConvertValue(valueStr, propertyType);
        var constant = Expression.Constant(convertedValue, propertyType);
        return BuildComparisonExpression(property, constant, op);
      }
    }
    else
    {
      // Если текущее свойство – коллекция (но не строка),
      // генерируем вызов Any(...) для дальнейшей фильтрации
      if (IsEnumerableButNotString(propertyType))
        return BuildCollectionExpression(property, members, index, op, valueStr);
      else
        // Если не коллекция – продолжаем цепочку навигации
        return BuildNestedExpression(property, members, index + 1, op, valueStr);
    }
  }

  static MethodCallExpression BuildCollectionExpression(Expression property,
                                                        string[] members,
                                                        int index,
                                                        FilterOperator op,
                                                        string valueStr)
  {
    var elementType = GetElementType(property.Type) 
      ?? throw new InvalidOperationException($"Couldn't define a collection item for the {property} type.");

    var lambdaParameter = Expression.Parameter(elementType, "e");
    var nested = BuildNestedExpression(lambdaParameter, members, index + 1, op, valueStr);
    var lambda = Expression.Lambda(nested, lambdaParameter);
    var anyMethod = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                                      .First(m => m.Name == "Any" && m.GetParameters().Length == 2)
                                      .MakeGenericMethod(elementType);

    return Expression.Call(anyMethod, property, lambda);
  }

  static object ConvertValue(string valueStr, Type targetType)
  {
    try
    {
      if (targetType == typeof(Guid))
        return Guid.Parse(valueStr);
      if (targetType.IsEnum)
        return Enum.Parse(targetType, valueStr, ignoreCase: true);
      return Convert.ChangeType(valueStr, targetType);
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"The value of '{valueStr}' couldn't be converted to the type '{targetType}'.", ex);
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
        FilterOperator.In => Expression.Call(right, GetInMethodInfo(left.Type), left),
        _ => throw new NotImplementedException($"Operator {op} is not supported.")
    };

  static MethodInfo GetContainsMethodInfo()
    => typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!;

  static MethodInfo GetInMethodInfo(Type elementType) 
    => typeof(List<>).MakeGenericType(elementType)
                     .GetMethod("Contains", [elementType])!;
}

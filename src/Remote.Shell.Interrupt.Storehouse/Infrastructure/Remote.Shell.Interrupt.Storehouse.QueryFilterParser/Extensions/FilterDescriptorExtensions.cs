namespace Remote.Shell.Interrupt.Storehouse.QueryFilterParser.Extensions;

internal static class FilterDescriptorExtensions
{
  public static Expression<Func<T, bool>> ToExpression<T>(this FilterDescriptor filter)
  {
    ArgumentNullException.ThrowIfNull(filter);

    var parameter = Expression.Parameter(typeof(T), "entity");
    var propertyPathSegments = filter.PropertyPath.Split('.');

    var body = BuildPropertyAccessChain(parameter,
                                        propertyPathSegments,
                                        0,
                                        filter.Operator,
                                        filter.Value);

    return Expression.Lambda<Func<T, bool>>(body, parameter);
  }

  static Expression BuildPropertyAccessChain(Expression currentExpression,
                                             string[] propertyPathSegments,
                                             int currentIndex,
                                             FilterOperator filterOperator,
                                             string filterValue)
  {
    var property = Expression.PropertyOrField(currentExpression,
                                              propertyPathSegments[currentIndex]);
        
    if (IsFinalPropertySegment(propertyPathSegments, currentIndex))
      return BuildFinalComparison(property,
                                  filterOperator,
                                  filterValue);

    if (IsCollectionProperty(property.Type))
      return BuildCollectionFilterExpression(property,
                                             propertyPathSegments,
                                             currentIndex,
                                             filterOperator,
                                             filterValue);

    return BuildPropertyAccessChain(property,
                                    propertyPathSegments,
                                    currentIndex + 1,
                                    filterOperator,
                                    filterValue);
  }

  static Expression BuildNextExpressionSegment(Expression currentExpression,
                                               string[] propertyPathSegments,
                                               int currentIndex,
                                               FilterOperator filterOperator,
                                               string filterValue)
  {
    var property = Expression.PropertyOrField(currentExpression,
                                              propertyPathSegments[currentIndex]);

    if (IsFinalPropertySegment(propertyPathSegments, currentIndex))
      return BuildFinalComparison(property,
                                  filterOperator,
                                  filterValue);

    if (IsCollectionProperty(property.Type))
      return BuildCollectionFilterExpression(property,
                                             propertyPathSegments,
                                             currentIndex,
                                             filterOperator,
                                             filterValue);

    return property;
  }

  static bool IsFinalPropertySegment(string[] segments, int index)
    => index == segments.Length - 1;

  static bool IsCollectionProperty(Type type)
    => typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string);

  static Expression BuildFinalComparison(Expression propertyExpression,
                                         FilterOperator filterOperator,
                                         string filterValue)
  {
    return filterOperator switch
    {
      FilterOperator.In => BuildInExpression(propertyExpression, filterValue),
      _ => BuildSimpleComparison(propertyExpression, filterOperator, filterValue)
    };
  }

  static Expression BuildInExpression(Expression propertyExpression,
                                      string filterValue)
  {
    var values = filterValue.Split(',')
                            .Select(value => ConvertFilterValue(value, propertyExpression.Type))
                            .ToList();

    var listType = typeof(List<>).MakeGenericType(propertyExpression.Type);
    var listInstance = (IList)Activator.CreateInstance(listType)!;

    foreach (var value in values)
      listInstance.Add(value);

    var constant = Expression.Constant(listInstance, listType);
    return Expression.Call(constant,
                           GetContainsMethod(propertyExpression.Type),
                           propertyExpression);
  }

  static Expression BuildSimpleComparison(Expression propertyExpression,
                                          FilterOperator filterOperator,
                                          string filterValue)
  {
    var convertedValue = ConvertFilterValue(filterValue, propertyExpression.Type);
    var constant = Expression.Constant(convertedValue, propertyExpression.Type);

    return filterOperator switch
    {
      FilterOperator.Equals => Expression.Equal(propertyExpression, constant),
      FilterOperator.NotEquals => Expression.NotEqual(propertyExpression, constant),
      FilterOperator.GraterThan => Expression.GreaterThan(propertyExpression, constant),
      FilterOperator.LessThan => Expression.LessThan(propertyExpression, constant),
      FilterOperator.Contains => Expression.Call(propertyExpression,
                                                 GetStringContainsMethod(),
                                                 constant),
      _ => throw new NotImplementedException($"Operator {filterOperator} is not supported.")
    };
  }

  static MethodCallExpression BuildCollectionFilterExpression(Expression collectionExpression,
                                                              string[] propertyPathSegments,
                                                              int currentIndex,
                                                              FilterOperator filterOperator,
                                                              string filterValue)
  {
    var elementType = GetCollectionElementType(collectionExpression.Type)
      ?? throw new InvalidOperationException($"Could not determine element type for collection {collectionExpression}");

    var parameter = Expression.Parameter(elementType, "element");
    var nestedExpression = BuildPropertyAccessChain(parameter,
                                                    propertyPathSegments,
                                                    currentIndex + 1,
                                                    filterOperator,
                                                    filterValue);

    var predicate = Expression.Lambda(nestedExpression, parameter);
    var anyMethod = GetEnumerableAnyMethod(elementType);

    return Expression.Call(anyMethod, collectionExpression, predicate);
  }

  static object ConvertFilterValue(string value, Type targetType)
  {
    try
    {
      if (targetType == typeof(Guid))
        return Guid.Parse(value);

      if (targetType.IsEnum)
        return Enum.Parse(targetType, value, ignoreCase: true);

      return Convert.ChangeType(value, targetType);
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Failed to convert value '{value}' to type '{targetType.Name}'.", ex);
    }
  }

  static Type GetCollectionElementType(Type collectionType)
  {
    if (collectionType.IsArray)
      return collectionType.GetElementType()!;

    if (collectionType.IsGenericType)
      return collectionType.GetGenericArguments()[0];

    return null!;
  }

  static MethodInfo GetEnumerableAnyMethod(Type elementType)
    => typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                         .First(m => m.Name == "Any" && m.GetParameters().Length == 2)
                         .MakeGenericMethod(elementType);

  static MethodInfo GetStringContainsMethod()
    => typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!;

  static MethodInfo GetContainsMethod(Type elementType)
    => typeof(List<>).MakeGenericType(elementType)
                     .GetMethod("Contains", [elementType])!;
}

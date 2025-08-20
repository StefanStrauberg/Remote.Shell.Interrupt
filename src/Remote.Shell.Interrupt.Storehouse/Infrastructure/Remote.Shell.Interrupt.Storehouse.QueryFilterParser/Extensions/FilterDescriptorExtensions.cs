namespace Remote.Shell.Interrupt.Storehouse.QueryFilterParser.Extensions;

internal static class FilterDescriptorExtensions
{
  public static Expression<Func<T, bool>> ToExpression<T>(this FilterDescriptor filter)
  {
    ArgumentNullException.ThrowIfNull(filter);

    var parameter = Expression.Parameter(typeof(T), "entity");
    var segments = filter.PropertyPath.Split('.');

    var body = BuildPropertyAccessChain(parameter, segments, 0, filter.Operator, filter.Value);

    return Expression.Lambda<Func<T, bool>>(body, parameter);
  }

  static Expression BuildPropertyAccessChain(Expression current,
                                             string[] segments,
                                             int index,
                                             FilterOperator op,
                                             string value)
  {
    var property = Expression.PropertyOrField(current, segments[index]);
        
    if (IsFinalPropertySegment(segments, index))
      return BuildFinalComparison(property, op, value);

    if (IsCollectionProperty(property.Type))
      return BuildCollectionFilterExpression(property, segments, index, op, value);

    return BuildPropertyAccessChain(property, segments, index + 1, op, value);
  }

  static bool IsFinalPropertySegment(string[] segments, int index)
    => index == segments.Length - 1;

  static bool IsCollectionProperty(Type type)
    => typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string);

  static Expression BuildFinalComparison(Expression property, FilterOperator op, string value)
    => op switch
    {
      FilterOperator.In => BuildInExpression(property, value),
      _ => BuildSimpleComparison(property, op, value)
    };

  static MethodCallExpression BuildInExpression(Expression property,
                                                string value)
  {
    var values = value.Split(',')
                      .Select(v => ConvertFilterValue(v, property.Type))
                      .ToList();

    var listType = typeof(List<>).MakeGenericType(property.Type);
    var list = (IList)Activator.CreateInstance(listType)!;

    foreach (var v in values)
      list.Add(v);

    var constant = Expression.Constant(list, listType);
    return Expression.Call(constant, GetContainsMethod(property.Type), property);
  }

  static Expression BuildSimpleComparison(Expression property, FilterOperator op, string value)
  {
    var converted = ConvertFilterValue(value, property.Type);
    var constant = Expression.Constant(converted, property.Type);

    return op switch
    {
      FilterOperator.Equals => Expression.Equal(property, constant),
      FilterOperator.NotEquals => Expression.NotEqual(property, constant),
      FilterOperator.GraterThan => Expression.GreaterThan(property, constant),
      FilterOperator.LessThan => Expression.LessThan(property, constant),
      FilterOperator.Contains => Expression.Call(property, GetStringContainsMethod(), constant),
      _ => throw new NotImplementedException($"Operator {op} is not supported.")
    };
  }

  static MethodCallExpression BuildCollectionFilterExpression(Expression collection, string[] segments, int index, FilterOperator op, string value)
  {
    var elementType = GetCollectionElementType(collection.Type)
      ?? throw new InvalidOperationException($"Could not determine element type for collection {collection}");

    var parameter = Expression.Parameter(elementType, "element");
    var nested = BuildPropertyAccessChain(parameter, segments, index + 1, op, value);

    var predicate = Expression.Lambda(nested, parameter);
    var anyMethod = GetEnumerableAnyMethod(elementType);

    return Expression.Call(anyMethod, collection, predicate);
  }

  static object ConvertFilterValue(string value, Type targetType)
  {
    try
    {
      if (targetType == typeof(Guid)) return Guid.Parse(value);
      if (targetType.IsEnum) return Enum.Parse(targetType, value, true);

      return Convert.ChangeType(value, targetType);
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Failed to convert value '{value}' to type '{targetType.Name}'.", ex);
    }
  }

  static Type? GetCollectionElementType(Type collectionType)
  {
    if (collectionType.IsArray) return collectionType.GetElementType();
    if (collectionType.IsGenericType) return collectionType.GetGenericArguments()[0];

    return null;
  }

  static MethodInfo GetEnumerableAnyMethod(Type elementType)
    => typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                         .First(m => m.Name == nameof(Enumerable.Any) && m.GetParameters().Length == 2)
                         .MakeGenericMethod(elementType);

  static MethodInfo GetStringContainsMethod()
    => typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!;

  static MethodInfo GetContainsMethod(Type elementType)
    => typeof(List<>).MakeGenericType(elementType)
                     .GetMethod(nameof(List<object>.Contains), [elementType])!;
}

namespace Remote.Shell.Interrupt.Storehouse.QueryFilterParser.Extensions;

internal static class FilterDescriptorExtensions
{
  /// <summary>
  /// Converts a <see cref="FilterDescriptor"/> into a LINQ expression for filtering entities of type <typeparamref name="T"/>.
  /// </summary>
  /// <typeparam name="T">The entity type to apply the filter against.</typeparam>
  /// <param name="filter">The filter descriptor containing property path, operator, and value.</param>
  /// <returns>
  /// A lambda expression representing the filter logic, suitable for use in LINQ queries.
  /// </returns>
  /// <exception cref="ArgumentNullException">
  /// Thrown when the <paramref name="filter"/> is <c>null</c>.
  /// </exception>
  /// <remarks>
  /// Supports nested property paths and collection navigation (e.g., <c>Ports.VLANs.Tag</c>).
  /// Dispatches to operator-specific logic including equality, comparison, and string-based matching.
  /// </remarks>
  public static Expression<Func<T, bool>> ToExpression<T>(this FilterDescriptor filter)
  {
    if (filter is null)
      throw new ArgumentNullException(nameof(filter), "Filter descriptor cannot be null when building an expression.");

    var parameter = Expression.Parameter(typeof(T), "entity");
    var segments = filter.PropertyPath.Split('.');

    var body = BuildPropertyAccessChain(parameter, segments, 0, filter.Operator, filter.Value);

    return Expression.Lambda<Func<T, bool>>(body, parameter);
  }

  /// <summary>
  /// Recursively builds a property access chain from a segmented path, applying the filter operator at the final segment.
  /// </summary>
  /// <param name="current">The current expression in the chain (starting with the root parameter).</param>
  /// <param name="segments">The split property path segments.</param>
  /// <param name="index">The current index in the segment traversal.</param>
  /// <param name="op">The filter operator to apply.</param>
  /// <param name="value">The filter value as a string.</param>
  /// <returns>
  /// An expression representing the full property access and comparison logic.
  /// </returns>
  /// <remarks>
  /// Supports nested properties and collection navigation. Delegates to <see cref="BuildFinalComparison"/> or <see cref="BuildCollectionFilterExpression"/> as needed.
  /// </remarks>
  static Expression BuildPropertyAccessChain(Expression current, string[] segments, int index, FilterOperator op, string value)
  {
    var property = Expression.PropertyOrField(current, segments[index]);

    if (IsFinalPropertySegment(segments, index))
      return BuildFinalComparison(property, op, value);

    if (IsCollectionProperty(property.Type))
      return BuildCollectionFilterExpression(property, segments, index, op, value);

    return BuildPropertyAccessChain(property, segments, index + 1, op, value);
  }

  /// <summary>
  /// Determines whether the current segment is the final property in the path.
  /// </summary>
  /// <param name="segments">The full array of property path segments.</param>
  /// <param name="index">The current index being evaluated.</param>
  /// <returns><c>true</c> if the current segment is the last; otherwise, <c>false</c>.</returns>
  static bool IsFinalPropertySegment(string[] segments, int index)
    => index == segments.Length - 1;

  /// <summary>
  /// Checks whether a given type represents a collection (excluding strings).
  /// </summary>
  /// <param name="type">The type to evaluate.</param>
  /// <returns><c>true</c> if the type is a collection; otherwise, <c>false</c>.</returns>
  static bool IsCollectionProperty(Type type)
    => typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string);

  /// <summary>
  /// Builds the final comparison expression based on the filter operator and value.
  /// </summary>
  /// <param name="property">The property expression to compare.</param>
  /// <param name="op">The filter operator.</param>
  /// <param name="value">The filter value as a string.</param>
  /// <returns>
  /// An expression representing the comparison logic (e.g., equality, string match, or inclusion).
  /// </returns>
  /// <remarks>
  /// Dispatches to either <see cref="BuildInExpression"/> or <see cref="BuildSimpleComparison"/>.
  /// </remarks>
  static Expression BuildFinalComparison(Expression property, FilterOperator op, string value)
    => op switch
    {
      FilterOperator.In => BuildInExpression(property, value),
      _ => BuildSimpleComparison(property, op, value)
    };

  /// <summary>
  /// Builds an expression that checks whether a property value is contained within a list of values.
  /// </summary>
  /// <param name="property">The property to evaluate.</param>
  /// <param name="value">A comma-separated string of values.</param>
  /// <returns>
  /// A method call expression representing <c>list.Contains(property)</c>.
  /// </returns>
  /// <exception cref="InvalidOperationException">
  /// Thrown if value conversion fails.
  /// </exception>
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

  /// <summary>
  /// Builds a simple comparison expression between a property and a constant value.
  /// </summary>
  /// <param name="property">The property to compare.</param>
  /// <param name="op">The comparison operator.</param>
  /// <param name="value">The filter value as a string.</param>
  /// <returns>
  /// An expression representing the comparison (e.g., <c>property == value</c>, <c>property.Contains(value)</c>).
  /// </returns>
  /// <exception cref="InvalidOperationException">
  /// Thrown if the operator is incompatible with the property type.
  /// </exception>
  static Expression BuildSimpleComparison(Expression property, FilterOperator op, string value)
  {
    if (property.Type != typeof(string) && (op == FilterOperator.Contains ||
                                            op == FilterOperator.Word ||
                                            op == FilterOperator.StartsWith ||
                                            op == FilterOperator.EndsWith))
      throw new InvalidOperationException($"Operator {op} can only be used with string properties. Property type: {property.Type.Name}");

    var converted = ConvertFilterValue(value, property.Type);
    var constant = Expression.Constant(converted, property.Type);

    return op switch
    {
      FilterOperator.Equals => Expression.Equal(property, constant),
      FilterOperator.NotEquals => Expression.NotEqual(property, constant),
      FilterOperator.GraterThan => Expression.GreaterThan(property, constant),
      FilterOperator.LessThan => Expression.LessThan(property, constant),
      FilterOperator.Contains => Expression.Call(property, GetMethodInfoOfContainsMethod(), constant),
      FilterOperator.Word => Expression.Call(null, GetMethodInfoOfContainsWholeWordMethod(), property, constant),
      FilterOperator.StartsWith => Expression.Call(property, GetMethodInfoOfStartsWithMethod(), constant),
      FilterOperator.EndsWith => Expression.Call(property, GetMethodInfoOfEndsWithMethod(), constant),
      _ => throw new NotImplementedException($"Operator {op} is not supported.")
    };
  }

  /// <summary>
  /// Builds a filter expression for a collection property using <c>Any</c> and a nested predicate.
  /// </summary>
  /// <param name="collection">The expression representing the collection property.</param>
  /// <param name="segments">The full property path segments.</param>
  /// <param name="index">The current segment index within the path.</param>
  /// <param name="op">The filter operator to apply.</param>
  /// <param name="value">The filter value as a string.</param>
  /// <returns>
  /// A method call expression representing <c>collection.Any(x => x.Property == value)</c>.
  /// </returns>
  /// <exception cref="InvalidOperationException">
  /// Thrown when the element type of the collection cannot be determined.
  /// </exception>
  /// <remarks>
  /// This method enables filtering on nested collections by recursively building property access chains.
  /// </remarks>
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

  /// <summary>
  /// Converts a string filter value to the target property type.
  /// </summary>
  /// <param name="value">The raw string value.</param>
  /// <param name="targetType">The type to convert to.</param>
  /// <returns>The converted value as an object.</returns>
  /// <exception cref="InvalidOperationException">
  /// Thrown if conversion fails.
  /// </exception>
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

  /// <summary>
  /// Retrieves the element type of a collection (array or generic enumerable).
  /// </summary>
  /// <param name="collectionType">The collection type.</param>
  /// <returns>The element type, or <c>null</c> if undeterminable.</returns>
  static Type? GetCollectionElementType(Type collectionType)
  {
    if (collectionType.IsArray) return collectionType.GetElementType();
    if (collectionType.IsGenericType) return collectionType.GetGenericArguments()[0];

    return null;
  }

  /// <summary>
  /// Gets the generic <c>Enumerable.Any</c> method for a given element type.
  /// </summary>
  /// <param name="elementType">The type of elements in the collection.</param>
  /// <returns>The method info for <c>Enumerable.Any&lt;T&gt;(IEnumerable&lt;T&gt;, Func&lt;T, bool&gt;)</c>.</returns>
  static MethodInfo GetEnumerableAnyMethod(Type elementType)
    => typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                         .First(m => m.Name == nameof(Enumerable.Any) && m.GetParameters().Length == 2)
                         .MakeGenericMethod(elementType);

  /// <summary>
  /// Gets the method info for <c>string.Contains(string)</c>.
  /// </summary>
  /// <returns>The method info object.</returns>
  static MethodInfo GetMethodInfoOfContainsMethod()
    => typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!;

  /// <summary>
  /// Gets the method info for <c>StringExtensions.ContainsWholeWord(string, string)</c>.
  /// </summary>
  /// <returns>The method info object.</returns>
  /// <exception cref="InvalidOperationException">
  /// Thrown if the method cannot be found via reflection.
  /// </exception>
  static MethodInfo GetMethodInfoOfContainsWholeWordMethod()
    => typeof(StringExtensions).GetMethod(nameof(StringExtensions.ContainsWholeWord),
                                                 BindingFlags.Static | BindingFlags.Public,
                                                 null,
                                                 [typeof(string), typeof(string)],
                                                 null)
      ?? throw new InvalidOperationException("ContainsWholeWord method not found");

  /// <summary>
  /// Gets the <c>Contains</c> method for a generic <c>List&lt;T&gt;</c>.
  /// </summary>
  /// <param name="elementType">The type of elements in the list.</param>
  /// <returns>The method info for <c>List&lt;T&gt;.Contains(T)</c>.</returns>
  static MethodInfo GetContainsMethod(Type elementType)
    => typeof(List<>).MakeGenericType(elementType)
                     .GetMethod(nameof(List<object>.Contains), [elementType])!;

  /// <summary>
  /// Gets the method info for <c>string.StartsWith(string)</c>.
  /// </summary>
  /// <returns>The method info object.</returns>
  static MethodInfo GetMethodInfoOfStartsWithMethod()
    => typeof(string).GetMethod(nameof(string.StartsWith), [typeof(string)])!;

  /// <summary>
  /// Gets the method info for <c>string.EndsWith(string)</c>.
  /// </summary>
  /// <returns>The method info object.</returns>
  static MethodInfo GetMethodInfoOfEndsWithMethod()
    => typeof(string).GetMethod(nameof(string.EndsWith), [typeof(string)])!;
}

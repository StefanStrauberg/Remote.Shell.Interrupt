namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Helpers;

internal static class GetUpdateProperties
{
  internal static string Handle<T>(bool excludeKey = false)
  {
    StringBuilder sb = new();
    Type type = typeof(T);

    // Получаем все свойства типа T, включая унаследованные
    PropertyInfo[] properties = type.GetProperties(BindingFlags.Public |
                                                   BindingFlags.NonPublic |
                                                   BindingFlags.Instance |
                                                   BindingFlags.Static |
                                                   BindingFlags.FlattenHierarchy);

    // Извлекаем имена свойств
    for (int i = 0; i < properties.Length; i++)
    {
      var property = properties[i];

      // Проверяем, является ли свойство классом или коллекцией
      bool isClass = property.PropertyType.IsClass && property.PropertyType != typeof(string);
      bool isCollection = typeof(System.Collections.IEnumerable).IsAssignableFrom(property.PropertyType) && property.PropertyType != typeof(string);

      // Если свойство является классом или коллекцией, пропускаем его
      if (isClass || isCollection)
        continue;

      // Проверяем, является ли свойство Id
      bool isGuuid = property.Name.Equals(nameof(BaseEntity.Id));

      // Если свойство является GUUID и excludeKey = true, пропускаем его
      if (excludeKey && isGuuid)
        continue;

      // Если свойство CreatedAt, пропускаем его
      if (property.Name.Equals(nameof(BaseEntity.CreatedAt)))
        continue;

      // Если свойство UpdatedAt, пропускаем его
      if (property.Name.Equals(nameof(BaseEntity.UpdatedAt)))
        continue;

      // Используем интерполяцию строк для формирования записи
      sb.Append($"\"{property.Name}\"=@{property.Name}");

      sb.Append(", ");
    }

    if (sb[^1] == ' ')
      sb.Length -= 2;

    return sb.ToString();
  }
}

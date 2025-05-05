namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Helpers;

internal static class GetTableName
{
  public static string Handle<K>()
  {
    StringBuilder sb = new();
    var name = typeof(K).Name;

    if (name[^1] == 'y')
    {
      sb.Append(name, 0, name.Length - 1); // Добавляем все символы, кроме последнего
      sb.Append("ies");
    }
    else
    {
      sb.Append(name);
      sb.Append('s');
    }

    return sb.ToString();
  }
  
  public static string Handle(string name)
  {
    StringBuilder sb = new();

    if (name[^1] == 'y')
    {
      sb.Append(name, 0, name.Length - 1); // Добавляем все символы, кроме последнего
      sb.Append("ies");
    }
    else
    {
      sb.Append(name);
      sb.Append('s');
    }

    return sb.ToString();
  }
}

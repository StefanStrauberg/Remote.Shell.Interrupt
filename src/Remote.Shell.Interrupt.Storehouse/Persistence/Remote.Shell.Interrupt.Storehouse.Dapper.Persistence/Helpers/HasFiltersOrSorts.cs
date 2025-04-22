namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Helpers;

internal static class HasFiltersOrSorts
{
  public static bool Handle(RequestParameters requestParameters)
  {
    return !string.IsNullOrEmpty(requestParameters.Filters) || 
           !string.IsNullOrEmpty(requestParameters.Sorts);
  }
}

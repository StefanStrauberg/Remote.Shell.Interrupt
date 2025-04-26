namespace Remote.Shell.Interrupt.Storehouse.Application.Models.Request;

public class RequestParametersUpdated
{
  public List<FilterDescriptor> Filters { get; set; } = [];
  public bool IncludeRelations { get; set; }
  public int PageNumber { get; set; }
  public int PageSize {  get; set; }
}

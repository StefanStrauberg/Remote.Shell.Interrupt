namespace Remote.Shell.Interrupt.Storehouse.Application.Models.Request;

public class RequestParametersUpdated
{
  public List<FilterDescriptor>? Filters { get; set; } = [];
  public int PageNumber { get; set; } = 1;
  public int PageSize {  get; set; } = 10;
}

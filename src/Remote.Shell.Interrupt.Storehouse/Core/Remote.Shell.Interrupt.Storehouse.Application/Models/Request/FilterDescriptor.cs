namespace Remote.Shell.Interrupt.Storehouse.Application.Models.Request;

public class FilterDescriptor
{
  public string PropertyPath { get; set; } = string.Empty;
  public FilterOperator Operator { get; set; }
  public string Value { get; set; } = string.Empty;
}

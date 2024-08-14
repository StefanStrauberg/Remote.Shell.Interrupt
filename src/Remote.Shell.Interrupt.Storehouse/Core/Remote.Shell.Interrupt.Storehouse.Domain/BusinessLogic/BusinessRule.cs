using System.Linq.Expressions;

namespace Remote.Shell.Interrupt.Storehouse.Domain.BusinessLogic;

public class BusinessRule : BaseEntity
{
  public string RuleName { get; set; } = string.Empty;
  public Expression<Func<object, bool>>? Condition { get; set; }
  // ToDo pyaload for OIDs
}

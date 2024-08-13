namespace Remote.Shell.Interrupt.Storehouse.Domain.Common;

public abstract class BaseEntity
{
  public Guid Id { get; private set; }
  public DateTime Created { get; private set; }
  public DateTime Modified { get; private set; }
}

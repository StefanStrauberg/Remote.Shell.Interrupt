namespace Remote.Shell.Interrupt.Storehouse.Domain;

public class Gateway : BaseEntity
{
  public required IPAddress Host { get; set; }
}

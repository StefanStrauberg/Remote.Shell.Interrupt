namespace Remote.Shell.Interrupt.Storehouse.Domain;

public abstract class Gateway : BaseEntity
{
  public GatewayLevel GatewayLevel { get; set; }
}

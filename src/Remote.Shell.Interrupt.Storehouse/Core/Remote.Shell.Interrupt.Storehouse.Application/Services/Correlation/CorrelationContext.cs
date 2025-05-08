namespace Remote.Shell.Interrupt.Storehouse.Application.Services.Correlation;

public class CorrelationContext : ICorrelationContext
{
  public string CorrelationId { get; set; } = Guid.NewGuid().ToString();
}

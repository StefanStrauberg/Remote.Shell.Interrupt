namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Correlation;

public interface ICorrelationContext
{
  string CorrelationId { get; set; }
}

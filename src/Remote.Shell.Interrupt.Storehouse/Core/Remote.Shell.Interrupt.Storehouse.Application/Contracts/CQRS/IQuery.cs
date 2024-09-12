namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.CQRS;

public interface IQuery<out TResponse> : IRequest<TResponse>
  where TResponse : notnull
{ }
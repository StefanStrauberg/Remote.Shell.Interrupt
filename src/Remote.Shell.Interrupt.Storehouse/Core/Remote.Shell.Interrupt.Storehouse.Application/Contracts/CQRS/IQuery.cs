namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.CQRS;

/// <summary>
/// Represents a query in the CQRS pattern that returns a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TResponse">The type of the response returned by the query.</typeparam>
public interface IQuery<out TResponse> : IRequest<TResponse>
  where TResponse : notnull
{ }

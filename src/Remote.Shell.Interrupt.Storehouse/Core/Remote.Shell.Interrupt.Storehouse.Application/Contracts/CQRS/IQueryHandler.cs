namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.CQRS;

/// <summary>
/// Represents a handler for a query in the CQRS pattern that returns a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TQuery">The type of the query being handled.</typeparam>
/// <typeparam name="TResponse">The type of the response returned by the query.</typeparam>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
  where TQuery : IRequest<TResponse>
  where TResponse : notnull
{ }

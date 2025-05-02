namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.CQRS;

/// <summary>
/// Represents a command in the CQRS pattern that does not return a response.
/// </summary>
public interface ICommand : ICommand<Unit>
{ }

/// <summary>
/// Represents a command in the CQRS pattern that returns a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TResponse">The type of the response returned by the command.</typeparam>
public interface ICommand<out TResponse> : IRequest<TResponse>
{ }
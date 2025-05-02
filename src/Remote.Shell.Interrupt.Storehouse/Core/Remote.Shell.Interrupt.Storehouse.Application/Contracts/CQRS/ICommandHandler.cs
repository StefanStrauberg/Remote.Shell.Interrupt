namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.CQRS;

/// <summary>
/// Represents a handler for a command in the CQRS pattern that does not return a response.
/// </summary>
/// <typeparam name="TCommand">The type of the command being handled.</typeparam>
public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand, Unit>
  where TCommand : ICommand<Unit>
{ }

/// <summary>
/// Represents a handler for a command in the CQRS pattern that returns a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TCommand">The type of the command being handled.</typeparam>
/// <typeparam name="TResponse">The type of the response returned by the command.</typeparam>
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
  where TCommand : ICommand<TResponse>
  where TResponse : notnull
{ }
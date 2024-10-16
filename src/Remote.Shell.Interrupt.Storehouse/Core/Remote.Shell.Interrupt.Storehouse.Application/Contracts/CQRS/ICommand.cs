namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.CQRS;

public interface ICommand : ICommand<Unit>
{ }

public interface ICommand<out TResponse> : IRequest<TResponse>
{ }
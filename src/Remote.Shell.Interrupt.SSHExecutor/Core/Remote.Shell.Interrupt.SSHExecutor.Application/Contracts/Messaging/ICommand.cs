namespace Remote.Shell.Interrupt.SSHExecutor.Application.Contracts.Messaging;

internal interface ICommand<out TResponse>
    : IRequest<TResponse>
{
}

namespace Remote.Shell.Interrupt.SNMPExecutor.Application.Contracts.Messaging;

internal interface ICommand<out TResponse>
    : IRequest<TResponse>
{
}

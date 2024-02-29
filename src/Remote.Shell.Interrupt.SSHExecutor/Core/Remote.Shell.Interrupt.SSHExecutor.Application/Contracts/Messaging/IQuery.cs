namespace Remote.Shell.Interrupt.SSHExecutor.Application.Contracts.Messaging;

internal interface IQuery<out TResponse> 
    : IRequest<TResponse>
{
}

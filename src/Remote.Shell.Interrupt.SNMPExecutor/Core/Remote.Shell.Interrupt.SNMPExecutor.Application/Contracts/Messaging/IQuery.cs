namespace Remote.Shell.Interrupt.SNMPExecutor.Application.Contracts.Messaging;

internal interface IQuery<out TResponse> 
    : IRequest<TResponse>
{
}

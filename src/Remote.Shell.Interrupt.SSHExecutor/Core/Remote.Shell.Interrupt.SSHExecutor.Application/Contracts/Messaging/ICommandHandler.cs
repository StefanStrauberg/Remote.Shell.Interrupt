namespace Remote.Shell.Interrupt.SSHExecutor.Application.Contracts.Messaging;

internal interface ICommandHandler<in TComand, TResponse> 
    : IRequestHandler<TComand, TResponse>
    where TComand : ICommand<TResponse>
{
}

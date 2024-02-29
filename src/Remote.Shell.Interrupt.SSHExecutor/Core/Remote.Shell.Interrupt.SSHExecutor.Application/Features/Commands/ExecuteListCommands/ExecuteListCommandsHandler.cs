
namespace Remote.Shell.Interrupt.SSHExecutor.Application.Features.Commands.ExecuteListCommands;

internal class ExecuteListCommandsHandler(IAppLogger<ExecuteListCommandsHandler> logger,
                                          ICommandExecutor executor) : ICommandHandler<ExecuteListCommands, Response>
{
    readonly IAppLogger<ExecuteListCommandsHandler> logger = logger
        ?? throw new ArgumentNullException(nameof(logger));
    readonly ICommandExecutor executor = executor
        ?? throw new ArgumentNullException(nameof(executor));
        
    Task<Response> IRequestHandler<ExecuteListCommands, Response>.Handle(ExecuteListCommands request,
                                                                         CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

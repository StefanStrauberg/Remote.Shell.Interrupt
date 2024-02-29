namespace Remote.Shell.Interrupt.SSHExecutor.Application.Features.Commands.ExecuteListCommands;

internal class ExecuteListCommandsHandler(IAppLogger<ExecuteListCommandsHandler> logger,
                                          ICommandExecutor executor) : ICommandHandler<ExecuteListCommands, Response>
{
    readonly IAppLogger<ExecuteListCommandsHandler> logger = logger
        ?? throw new ArgumentNullException(nameof(logger));
    readonly ICommandExecutor executor = executor
        ?? throw new ArgumentNullException(nameof(executor));

    async Task<Response> IRequestHandler<ExecuteListCommands, Response>.Handle(ExecuteListCommands request,
                                                                               CancellationToken cancellationToken)
    {
        var response = await executor.ExecuteCommands(request.ServerParams,
                                                      request.Commands);
        foreach (var command in request.Commands)
        {    
            logger.LogInformation($"{DateTime.Now} - " +
                                  $"{request.ServerParams.UserName}@{request.ServerParams.HostName} " +
                                  $"executed: {command}");
        }
        return response;
    }
}

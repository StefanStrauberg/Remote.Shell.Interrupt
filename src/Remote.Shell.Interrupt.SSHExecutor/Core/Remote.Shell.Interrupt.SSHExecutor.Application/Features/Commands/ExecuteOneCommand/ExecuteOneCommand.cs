namespace Remote.Shell.Interrupt.SSHExecutor.Application.Features.Commands.ExecuteOneCommand;

/// <summary>
/// CQRS command to execut command on remote server
/// </summary>
public record class ExecuteOneCommand(ServerParams ServerParams,
                                      Command Command) 
    : ICommand<Response>;
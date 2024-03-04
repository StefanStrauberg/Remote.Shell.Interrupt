namespace Remote.Shell.Interrupt.SSHExecutor.Application.Features.SSHExecutor.Commands;

/// <summary>
/// CQRS command to execut commands on remote server
/// </summary>
public record class SSHExecuteCommands(SSHParams ServerParams,
                                       List<string> Commands) 
    : ICommand<string>;
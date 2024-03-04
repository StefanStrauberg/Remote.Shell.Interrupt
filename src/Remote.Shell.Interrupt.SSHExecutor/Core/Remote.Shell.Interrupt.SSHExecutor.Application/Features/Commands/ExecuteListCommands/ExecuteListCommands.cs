namespace Remote.Shell.Interrupt.SSHExecutor.Application.Features.Commands.ExecuteListCommands;

/// <summary>
/// CQRS command to execut commands on remote server
/// </summary>
public record class ExecuteListCommands(ServerParams ServerParams,
                                        List<string> Commands) 
    : ICommand<string>;
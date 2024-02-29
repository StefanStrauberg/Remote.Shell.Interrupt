namespace Remote.Shell.Interrupt.SSHExecutor.Application.Features.Commands.ExecuteListCommands;

/// <summary>
/// CQRS command to execut commands on remote server
/// </summary>
record class ExecuteListCommands(ServerParams ServerParams, List<Command> Commands) : ICommand<Response>;
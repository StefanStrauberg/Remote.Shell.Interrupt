namespace Remote.Shell.Interrupt.SSHExecutor.Application.Features.Commands.ExecuteOneCommand;

/// <summary>
/// CQRS command to execut command on remote server
/// </summary>
record class ExecuteOneCommand(Command Command) : ICommand<Response>;
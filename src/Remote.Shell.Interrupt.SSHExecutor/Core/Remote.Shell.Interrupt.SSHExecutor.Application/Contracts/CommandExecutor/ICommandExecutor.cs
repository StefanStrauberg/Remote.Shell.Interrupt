namespace Remote.Shell.Interrupt.SSHExecutor.Application.Contracts.CommandExecutor;

/// <summary>
/// Command Executor of SSH Executor Application Service
/// </summary>
public interface ICommandExecutor
{
    /// <summary>
    /// Delegate for logging of executed command
    /// </summary>
    /// <param name="message">Message returned after execution command</param>
    delegate void CommandExecutorHandler(string message, 
                                         params object[] args);
    /// <summary>
    /// Event for CommandExecutorHandler
    /// </summary>
    public event CommandExecutorHandler? Notify;

    /// <summary>
    /// Execut one command on remote server
    /// </summary>
    /// <param name="serverParams">Parameters of connection to server</param>
    /// <param name="command">Executable command</param>
    /// <param name="token">CancellationToken</param>
    /// <returns>Response - Response from remote server</returns>
    Task<Response> ExecuteCommand(ServerParams serverParams,
                                  Command command,
                                  CancellationToken cancellationToken);
    /// <summary>
    /// Execut many commands on remote server
    /// </summary>
    /// <param name="serverParams">Parameters of connection to server</param>
    /// <param name="commands">Executable command</param>
    /// <param name="token">CancellationToken</param>
    /// <returns>Response - Response from remote server</returns>
    Task<Response> ExecuteCommands(ServerParams serverParams,
                                   List<Command> commands,
                                   CancellationToken cancellationToken);
}

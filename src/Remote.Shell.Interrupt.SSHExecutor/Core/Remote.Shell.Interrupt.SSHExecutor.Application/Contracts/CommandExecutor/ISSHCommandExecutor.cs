namespace Remote.Shell.Interrupt.SSHExecutor.Application.Contracts.CommandExecutor;

/// <summary>
/// Interface for executing commands on remote client
/// </summary>
public interface ISSHCommandExecutor
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
    /// Execut commands on remote client
    /// </summary>
    /// <param name="serverParams">Parameters of connection to client</param>
    /// <param name="commands">Executable commands</param>
    /// <param name="token">CancellationToken</param>
    /// <returns>Responses from remote client after executing each command</returns>
    Task<string> ExecuteCommands(SSHParams sshParams,
                                 List<string> commands,
                                 CancellationToken cancellationToken);
}

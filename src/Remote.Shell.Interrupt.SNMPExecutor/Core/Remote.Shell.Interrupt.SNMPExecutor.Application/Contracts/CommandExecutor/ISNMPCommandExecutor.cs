namespace Remote.Shell.Interrupt.SNMPExecutor.Application.Contracts.CommandExecutor;

/// <summary>
/// Interface for executing commands on remote network device
/// </summary>
public interface ISNMPCommandExecutor
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
    /// Execut commands on remote network device
    /// </summary>
    /// <param name="serverParams">Parameters of connection to network device</param>
    /// <param name="commands">Executable command</param>
    /// <param name="token">CancellationToken</param>
    /// <returns>Responses from remote network device after executing each command</returns>
    Task<string> WalkCommand(SNMPParams snmpParams,
                             string oids,
                             CancellationToken cancellationToken);
}

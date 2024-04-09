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
    /// Execut Walk command on remote network device
    /// </summary>
    /// <param name="snmpParams">Parameters of connection to network device</param>
    /// <param name="oid">Executable command</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>Responses from remote network device after executing command</returns>
    Task<IList<Response>> WalkCommand(SNMPParams sNMPParams,
                                      string oid,
                                      CancellationToken cancellationToken);

    /// <summary>
    /// Execut Get command on remote network device
    /// </summary>
    /// <param name="sNMPParams">Parameters of connection to network device</param>
    /// <param name="oid">Executable command</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>Responses from remote network device after executing command</returns>
    Task<IList<Response>> GetCommand(SNMPParams sNMPParams,
                                     string oid,
                                     CancellationToken cancellationToken);
}

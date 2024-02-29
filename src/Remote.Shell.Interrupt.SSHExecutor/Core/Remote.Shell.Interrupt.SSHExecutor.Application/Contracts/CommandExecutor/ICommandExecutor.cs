namespace Remote.Shell.Interrupt.SSHExecutor.Application.Contracts.CommandExecutor;

/// <summary>
/// Command Executor of SSH Executor Application Service
/// </summary>
public interface ICommandExecutor
{
    /// <summary>
    /// Execut one command on remote server
    /// </summary>
    /// <param name="serverParams">Parameters of connection to server</param>
    /// <param name="command">Executable command</param>
    /// <returns>Response - Response from remote server</returns>
    Task<Response> ExecuteCommand(ServerParams serverParams, 
                                  Command command);
    /// <summary>
    /// Execut many commands on remote server
    /// </summary>
    /// <param name="serverParams">Parameters of connection to server</param>
    /// <param name="commands">Executable command</param>
    /// <returns>Response - Response from remote server</returns>
    Task<Response> ExecuteCommands(ServerParams serverParams, 
                                   List<Command> commands);
}

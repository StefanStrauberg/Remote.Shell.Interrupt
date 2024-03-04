using System.Text;

namespace Remote.Shell.Interrupt.SSHExecutor.CommandExecutor;

/// <summary>
/// Implementation of business logic of command executing 
/// </summary>
internal class CommandExecutor : ICommandExecutor
{
    public event ICommandExecutor.CommandExecutorHandler? Notify;

    /// <summary>
    /// Executing commands on remote server
    /// </summary>
    /// <param name="serverParams"></param>
    /// <param name="commands"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    async Task<string> ICommandExecutor.ExecuteCommands(ServerParams serverParams,
                                                        List<string> commands,
                                                        CancellationToken cancellationToken)
    {
        using var client = new SshClient(serverParams.HostName,
                                         serverParams.Port,
                                         serverParams.UserName,
                                         serverParams.Password);

        await client.ConnectAsync(cancellationToken);

        var sb = new StringBuilder();

        foreach (var command in commands)
        {
            var result = client.RunCommand(command);

            sb.Append(serverParams.UserName);
            sb.Append('@');
            sb.Append(serverParams.HostName + ' ');
            sb.Append("Command: ");
            sb.Append(command + ' ');
            sb.Append("Result: ");
            sb.Append(result.Result);
            sb.Append("ExitStatus: ");
            sb.Append(result.ExitStatus);
            sb.Append(" Error: ");
            sb.Append(result.Error);
            sb.Append('\xA');
        }

        var response = sb.ToString();

        Notify?.Invoke(response);

        client.Disconnect();

        return response;
    }
}

namespace Remote.Shell.Interrupt.SSHExecutor.CommandExecutor;

/// <summary>
/// Implementation of business logic of command executing 
/// </summary>
internal class SSHCommandExecutor : ISSHCommandExecutor
{
    public event ISSHCommandExecutor.CommandExecutorHandler? Notify;

    /// <summary>
    /// Executing commands on remote server
    /// </summary>
    /// <param name="sshParams"></param>
    /// <param name="commands"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    async Task<string> ISSHCommandExecutor.ExecuteCommands(SSHParams sshParams,
                                                        List<string> commands,
                                                        CancellationToken cancellationToken)
    {
        using var client = new SshClient(sshParams.HostName,
                                         sshParams.Port,
                                         sshParams.UserName,
                                         sshParams.Password);

        await client.ConnectAsync(cancellationToken);

        var sb = new StringBuilder();

        foreach (var command in commands)
        {
            var result = client.RunCommand(command);

            sb.Append(sshParams.UserName);
            sb.Append('@');
            sb.Append(sshParams.HostName + ' ');
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

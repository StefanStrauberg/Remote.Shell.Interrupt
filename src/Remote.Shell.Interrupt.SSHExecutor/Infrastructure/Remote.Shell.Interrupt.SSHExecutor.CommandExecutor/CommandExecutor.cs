using System.Text;

namespace Remote.Shell.Interrupt.SSHExecutor.CommandExecutor;

/// <summary>
/// Implementation of business logic of command executing 
/// </summary>
internal class CommandExecutor : ICommandExecutor
{
    public event ICommandExecutor.CommandExecutorHandler? Notify;

    /// <summary>
    /// Executing one command on remote server
    /// </summary>
    /// <param name="serverParams"></param>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    async Task<Response> ICommandExecutor.ExecuteCommand(ServerParams serverParams,
                                                         Command command,
                                                         CancellationToken cancellationToken)
    {
        using var client = new SshClient(serverParams.HostName,
                                         serverParams.UserName,
                                         serverParams.Password);

        await client.ConnectAsync(cancellationToken);

        var result = client.RunCommand(command.Line);

        Notify?.Invoke($"{DateTime.Now} - " +
                       $"{serverParams.UserName}@{serverParams.HostName} " +
                       $"Executed: {command.Line}" +
                       $"Result: {result.Result}" +
                       $"ExitStatus: {result.ExitStatus}" +
                       $"Error: {result.Error}");

        client.Disconnect();

        return new Response() { Line = result.Result };
    }

    /// <summary>
    /// Executing many commands on remote server
    /// </summary>
    /// <param name="serverParams"></param>
    /// <param name="commands"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    async Task<Response> ICommandExecutor.ExecuteCommands(ServerParams serverParams,
                                                          List<Command> commands,
                                                          CancellationToken cancellationToken)
    {
        using var client = new SshClient(serverParams.HostName,
                                         serverParams.UserName,
                                         serverParams.Password);

        await client.ConnectAsync(cancellationToken);

        List<SshCommand> results = [];

        foreach (var command in commands)
        {
            var commandResult = client.RunCommand(command.Line);

            Notify?.Invoke($"{DateTime.Now} - " +
                       $"{serverParams.UserName}@{serverParams.HostName} " +
                       $"Executed: {command.Line}" +
                       $"Result: {commandResult.Result}" +
                       $"ExitStatus: {commandResult.ExitStatus}" +
                       $"Error: {commandResult.Error}");
                       
            results.Add(commandResult);
        }

        client.Disconnect();

        StringBuilder result = new();

        foreach (var status in results)
            result.AppendLine(status.Result);

        return new Response() { Line = result.ToString() };
    }
}

namespace Remote.Shell.Interrupt.SSHExecutor.Domain;

/// <summary>
/// Response from Server after executing command
/// </summary>
public class SSHResponse
{
    /// <summary>
    /// Response message from server
    /// </summary>
    /// <value>Recieve string from server or default</value>
    public string UserName { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public string Executed { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    public int ExitStatus { get; set; }
    public string Error { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"{UserName}@{Host}\n"
            + $"Executed: {Executed}\n"
            + $"Result: {Result}\n"
            + $"ExitStatus: {ExitStatus}\n"
            + $"Error: {Error}";
    }
}

namespace Remote.Shell.Interrupt.SSHExecutor.Domain;

/// <summary>
/// Response from Server after executing command
/// </summary>
public class Response
{
    /// <summary>
    /// Response message from server
    /// </summary>
    /// <value>Recieve string from server or default</value>
    public string Line { get; set; } = string.Empty;
}

namespace Remote.Shell.Interrupt.SSHExecutor.Domain;

/// <summary>
/// Command that should be executing on remote server
/// </summary>
public class Command
{
    /// <summary>
    /// Command that should be executing
    /// </summary>
    /// <value>Commnd or default</value>
    public string Line { get; set; } = string.Empty;
}

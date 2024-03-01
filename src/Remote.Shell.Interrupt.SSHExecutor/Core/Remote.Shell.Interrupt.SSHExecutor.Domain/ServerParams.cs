namespace Remote.Shell.Interrupt.SSHExecutor.Domain;

/// <summary>
/// Parameters of connection to server
/// </summary>
public class ServerParams
{
    /// <summary>
    /// Hostname or IP Addrer of server
    /// </summary>
    /// <value></value>
    public string HostName { get; set; } = string.Empty;
    /// <summary>
    /// Remote port for connection
    /// </summary>
    /// <value></value>
    public int Port { get; set; } = 22;
    /// <summary>
    /// User name for connection to server
    /// </summary>
    /// <value></value>
    public string UserName { get; set; } = string.Empty;
    /// <summary>
    /// Password of user
    /// </summary>
    /// <value></value>
    public string Password { get; set; } = string.Empty;
}

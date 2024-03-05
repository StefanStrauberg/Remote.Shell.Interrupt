namespace Remote.Shell.Interrupt.SNMPExecutor.Domain;

/// <summary>
/// Parameters of connection to server
/// </summary>
public class SNMPParams
{
    /// <summary>
    /// Hostname or IP Addrer of server
    /// </summary>
    /// <value>Default - empty string</value>
    public string Host { get; set; } = string.Empty;
    /// <summary>
    /// Remote port for connection
    /// </summary>
    /// <value>Default - 161</value>
    public int Port { get; set; } = 161;
    /// <summary>
    /// Commynity of remote network device
    /// </summary>
    /// <value>Default - empty string</value>
    public string Community { get; set; } = string.Empty;
}

namespace Remote.Shell.Interrupt.SNMPExecutor.Domain;

/// <summary>
/// Parameters of connection to server
/// </summary>
public class SNMPParams(string host, string community)
{
    /// <summary>
    /// Hostname or IP Addrer of server
    /// </summary>
    /// <value>Default - empty string</value>
    public string Host { get; init; } = host;
    /// <summary>
    /// Commynity of remote network device
    /// </summary>
    /// <value>Default - empty string</value>
    public string Community { get; init; } = community;
}

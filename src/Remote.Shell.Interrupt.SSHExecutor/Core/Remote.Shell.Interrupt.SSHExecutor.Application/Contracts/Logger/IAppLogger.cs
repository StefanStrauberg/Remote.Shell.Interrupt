namespace Remote.Shell.Interrupt.SSHExecutor.Application.Contracts.Logger;

/// <summary>
/// Custom logic of Logger of SSH Executor Application Service
/// </summary>
/// <typeparam name="T">TypeOf T</typeparam>
public interface IAppLogger<T>
{
    /// <summary>
    /// Information log
    /// </summary>
    /// <param name="message">Message log</param>
    /// <param name="args">Arguments of message</param>
    void LogInformation(string message, 
                        params object[] args);
    /// <summary>
    /// Warning log
    /// </summary>
    /// <param name="message">Message log</param>
    /// <param name="args">Arguments of message</param>
    void LogWarning(string message, 
                    params object[] args);
}

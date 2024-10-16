namespace Remote.Shell.Interrupt.Storehouse.Application.Contracts.Logger;

public interface IAppLogger<T>
{
  void LogInformation(string message, params object[] args);

  void LogWarning(string message, params object[] args);

  void LogError(string message, params object[] args);
}

public interface IAppLogger
{
  void LogInformation(string className, string message, params object[] args);

  void LogWarning(string className, string message, params object[] args);

  void LogError(string className, string message, params object[] args);
}

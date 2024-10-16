namespace Remote.Shell.Interrupt.Storehouse.Application.Exceptions;

public abstract class ApplicationException : Exception
{
  public string Title { get; }
  protected ApplicationException(string title, string message)
        : base(message)
  {
    Title = title;
  }

  protected ApplicationException(string title, string message, Exception innerException)
      : base(message, innerException)
  {
    Title = title;
  }
}

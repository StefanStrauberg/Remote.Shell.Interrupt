namespace Remote.Shell.Interrupt.Storehouse.Application.Exceptions;

public class IncorrectInputData(string incorrectMessage)
  : BadRequestException(incorrectMessage)
{ }
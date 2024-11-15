namespace Remote.Shell.Interrupt.Storehouse.Application.Exceptions;

public class EntityAlreadyExists(string expression)
  : BadRequestException($"The Assignment with {expression} already exists.")
{ }

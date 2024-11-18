namespace Remote.Shell.Interrupt.Storehouse.Application.Exceptions;

public class EntityAlreadyExists(string expression)
  : BadRequestException($"The Entity with {expression} already exists.")
{ }
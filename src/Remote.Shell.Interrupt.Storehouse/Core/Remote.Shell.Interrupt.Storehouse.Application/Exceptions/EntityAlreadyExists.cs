namespace Remote.Shell.Interrupt.Storehouse.Application.Exceptions;

public class EntityAlreadyExists(string expression)
  : BadRequestException($"The entity with expression: \"{expression}\" already exists.")
{ }

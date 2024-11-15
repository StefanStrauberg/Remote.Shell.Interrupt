namespace Remote.Shell.Interrupt.Storehouse.Application.Exceptions;

public class EntityNotFoundException(string expression)
  : BadRequestException($"The entity with expression: \"{expression}\" was not found.")
{ }

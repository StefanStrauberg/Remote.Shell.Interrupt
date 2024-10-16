namespace Remote.Shell.Interrupt.Storehouse.Application.Exceptions;

public class EntityNotFoundException(string expression)
  : BadRequestException($"An entity with expression: \"{expression}\" was not found.")
{ }

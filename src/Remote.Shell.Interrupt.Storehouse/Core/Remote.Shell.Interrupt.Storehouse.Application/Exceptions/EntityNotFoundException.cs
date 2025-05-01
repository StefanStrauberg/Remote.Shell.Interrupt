namespace Remote.Shell.Interrupt.Storehouse.Application.Exceptions;

public class EntityNotFoundException(Type typeOfEntity, string expression)
  : NotFoundException($"The {typeOfEntity.Name} with expression: \"{expression}\" was not found.")
{ }

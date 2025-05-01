namespace Remote.Shell.Interrupt.Storehouse.Application.Exceptions;

public class EntityAlreadyExists(Type typeOfEntity, string expression)
  : BadRequestException($"The {typeOfEntity.Name} with expression: \"{expression}\" already exists.")
{ }
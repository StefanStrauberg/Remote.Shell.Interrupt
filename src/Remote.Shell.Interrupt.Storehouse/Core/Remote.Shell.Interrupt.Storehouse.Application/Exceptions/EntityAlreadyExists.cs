namespace Remote.Shell.Interrupt.Storehouse.Application.Exceptions;

/// <summary>
/// Represents an exception indicating that an entity already exists.
/// </summary>
/// <param name="typeOfEntity">The type of the entity that already exists.</param>
/// <param name="expression">The expression used to identify the entity.</param>
public class EntityAlreadyExists(Type typeOfEntity, string expression)
  : BadRequestException($"The {typeOfEntity.Name} with expression: \"{expression}\" already exists.")
{ }
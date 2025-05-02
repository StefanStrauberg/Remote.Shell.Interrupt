namespace Remote.Shell.Interrupt.Storehouse.Application.Exceptions;

/// <summary>
/// Represents an exception indicating that an entity was not found.
/// </summary>
/// <param name="typeOfEntity">The type of the entity that was not found.</param>
/// <param name="expression">The expression used to identify the entity.</param>
public class EntityNotFoundException(Type typeOfEntity, string expression)
  : NotFoundException($"The {typeOfEntity.Name} with expression: \"{expression}\" was not found.")
{ }

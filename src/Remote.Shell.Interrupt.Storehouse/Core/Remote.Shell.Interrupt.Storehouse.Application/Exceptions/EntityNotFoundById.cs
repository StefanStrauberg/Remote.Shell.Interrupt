namespace Remote.Shell.Interrupt.Storehouse.Application.Exceptions;

public class EntityNotFoundById(Type typeOfEntity, string id)
  : NotFoundException($"The {typeOfEntity.Name} with the Id: \"{id}\" was not found.")
{ }

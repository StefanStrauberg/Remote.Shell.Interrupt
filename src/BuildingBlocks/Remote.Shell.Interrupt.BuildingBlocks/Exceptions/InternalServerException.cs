namespace Remote.Shell.Interrupt.BuildingBlocks.Exceptions;

public class InternalServerException(string message) : ApplicationException("Internal Server Exception", message)
{ }

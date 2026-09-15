namespace Remote.Shell.Interrupt.Storehouse.Application.Exceptions;

/// <summary>
/// Thrown when a workflow status transition or graph edit is attempted from a status that
/// doesn't allow it (e.g. editing/publishing a workflow that isn't <c>Draft</c>, or
/// archiving one that's already <c>Archived</c>).
/// </summary>
public class WorkflowNotEditableException(string message)
  : BadRequestException(message)
{ }

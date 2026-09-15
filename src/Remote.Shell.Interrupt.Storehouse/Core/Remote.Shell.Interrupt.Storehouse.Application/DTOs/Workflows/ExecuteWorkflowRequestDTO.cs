namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Workflows;

/// <summary>
/// Target device to run a workflow against. Mirrors <c>SNMPGetCommand</c>'s
/// Host/Community fields rather than referencing a persisted Gate/NetworkDevice.
/// </summary>
public class ExecuteWorkflowRequestDTO
{
  public string Host { get; set; } = string.Empty;

  public string Community { get; set; } = string.Empty;
}

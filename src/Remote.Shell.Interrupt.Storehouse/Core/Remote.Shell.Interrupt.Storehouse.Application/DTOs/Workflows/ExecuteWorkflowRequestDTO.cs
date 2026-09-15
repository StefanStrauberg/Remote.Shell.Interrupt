namespace Remote.Shell.Interrupt.Storehouse.Application.DTOs.Workflows;

/// <summary>
/// Target device to run a workflow against. Mirrors <c>SNMPGetCommand</c>'s
/// Host/Community fields rather than referencing a persisted Gate/NetworkDevice.
/// </summary>
public class ExecuteWorkflowRequestDTO
{
  public string Host { get; set; } = string.Empty;

  public string Community { get; set; } = string.Empty;

  /// <summary>
  /// Optional context variables seeded before the run starts (e.g. a caller-supplied
  /// vendor a Decision node reads, rather than something detected via SNMP).
  /// </summary>
  public Dictionary<string, object?> Input { get; set; } = [];
}

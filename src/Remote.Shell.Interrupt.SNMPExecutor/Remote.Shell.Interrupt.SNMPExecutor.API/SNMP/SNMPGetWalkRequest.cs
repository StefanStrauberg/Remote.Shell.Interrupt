namespace Remote.Shell.Interrupt.SNMPExecutor.API.SNMP;

public record SNMPGetWalkRequest(string? Host, string? Community, string? OID);
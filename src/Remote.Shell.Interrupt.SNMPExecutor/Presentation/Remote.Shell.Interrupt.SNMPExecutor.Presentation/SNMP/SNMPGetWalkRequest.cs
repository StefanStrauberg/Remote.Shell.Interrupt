namespace Remote.Shell.Interrupt.SNMPExecutor.Presentation.SNMP;

public record SNMPGetWalkRequest(string Host, string Community, string OID);
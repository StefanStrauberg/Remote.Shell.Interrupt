namespace Remote.Shell.Interrupt.Storehouse.Application.Features.SNMPExecutor.Get;

public record SNMPGetCommand(string Host, string Community, string OID) : IRequest<SNMPResponse>;

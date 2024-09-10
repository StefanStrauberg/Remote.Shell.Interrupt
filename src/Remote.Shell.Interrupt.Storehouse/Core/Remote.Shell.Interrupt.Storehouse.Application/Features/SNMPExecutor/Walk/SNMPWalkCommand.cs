namespace Remote.Shell.Interrupt.Storehouse.Application.Features.SNMPExecutor.Walk;

public record class SNMPWalkCommand(string Host,
                                    string Community,
                                    string OID) : IRequest<IEnumerable<SNMPResponse>>;

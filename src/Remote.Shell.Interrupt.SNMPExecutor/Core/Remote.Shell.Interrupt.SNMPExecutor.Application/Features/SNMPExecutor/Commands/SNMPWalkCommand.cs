namespace Remote.Shell.Interrupt.SNMPExecutor.Application.Features.SNMPExecutor.Commands;

public record class SNMPWalkCommand(SNMPParams SNMPParams,
                                    string Command) 
    : ICommand<string>;

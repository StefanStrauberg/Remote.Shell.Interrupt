namespace Remote.Shell.Interrupt.SSHExecutor.Application.Features.SNMPExecutor.Commands;

public record class SNMPExecuteCommands(SNMPParams SNMPParams,
                                        List<string> Commands) 
    : ICommand<string>;

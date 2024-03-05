namespace Remote.Shell.Interrupt.SNMPExecutor.Application.DTOs;

public record class WalkCmdAndPrms(SNMPParams ServerParams,
                                   string Command);

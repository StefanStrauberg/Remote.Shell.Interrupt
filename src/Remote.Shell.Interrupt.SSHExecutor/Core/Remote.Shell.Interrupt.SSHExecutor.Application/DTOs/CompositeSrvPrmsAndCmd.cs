namespace Remote.Shell.Interrupt.SSHExecutor.Application.DTOs;

public record class CompositeSrvPrmsAndCmd(ServerParams ServerParams,
                                           Command Command);

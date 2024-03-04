namespace Remote.Shell.Interrupt.SSHExecutor.Application.DTOs;

public record class CompositeSrvPrmsAndCmds(ServerParams ServerParams,
                                            List<string> Commands);

namespace Remote.Shell.Interrupt.SSHExecutor.Application.DTOs;

public record class ComposSrvPrmsAndCmds(SSHParams ServerParams,
                                         List<string> Commands);

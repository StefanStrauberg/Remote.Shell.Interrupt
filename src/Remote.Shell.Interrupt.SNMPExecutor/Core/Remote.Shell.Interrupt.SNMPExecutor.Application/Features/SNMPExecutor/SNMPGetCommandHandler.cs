namespace Remote.Shell.Interrupt.SNMPExecutor.Application.Features.SNMPExecutor;

public record SNMPGetCommand(string Host,
                             string Community,
                             string OID) : IRequest<Info>;

public class SNMPGetCommandValidator : AbstractValidator<SNMPGetCommand>
{
    public SNMPGetCommandValidator()
    {
        RuleFor(x => x.Host).NotNull().WithMessage("Host can't be null")
                            .NotEmpty().WithMessage("Host can't be empty")
                            .Matches(@"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$")
                            .WithMessage("Invalid host. Host should be IPv4 address");
        RuleFor(x => x.Community).NotNull().WithMessage("Community can't be null")
                                 .NotEmpty().WithMessage("Community can't be empty")
                                 .WithMessage("Community is required");
        RuleFor(x => x.OID).NotNull().WithMessage("OID can't be null")
                           .NotEmpty().WithMessage("OID can't be empty")
                           .WithMessage("OID is required")
                           .Matches(@"^\d+(\.\d+)+$")
                           .WithMessage("Invalid OID. OID should be mtches x.x.x where x are numbers");
    }
}

internal class SNMPGetCommandHandler(ISNMPCommandExecutor executor) : IRequestHandler<SNMPGetCommand, Info>
{
    readonly ISNMPCommandExecutor _executor = executor ?? throw new ArgumentNullException(nameof(executor));

    async Task<Info> IRequestHandler<SNMPGetCommand, Info>.Handle(SNMPGetCommand request,
                                                                  CancellationToken cancellationToken)
    {
        return await _executor.GetCommand(request.Host,
                                          request.Community,
                                          request.OID,
                                          cancellationToken);
    }
}

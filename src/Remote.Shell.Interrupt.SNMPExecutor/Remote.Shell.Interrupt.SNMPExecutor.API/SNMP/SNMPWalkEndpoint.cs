namespace Remote.Shell.Interrupt.SNMPExecutor.API.SNMP;

public record SNMPWalkRequest(string Host,
                              string Community,
                              string OID);

public class SNMPGetRequestValidator : AbstractValidator<SNMPWalkRequest>
{
  public SNMPGetRequestValidator()
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
                       .WithMessage("Invalid OID. OID should be mtches x.x.x where x are numbers"); ;
  }
}

public record SNMPWalkResponse(string OID,
                               string TypeCode,
                               string Data);

public class SNMPWalkEndpoint : ICarterModule
{
  public void AddRoutes(IEndpointRouteBuilder app)
  {
    app.MapGet("/executor/walk", async (SNMPWalkRequest request, ISender sender) =>
    {
      var command = request.Adapt<SNMPWalkCommand>();
      var result = await sender.Send(command);
      var response = result.Adapt<SNMPWalkResponse>();
      return Results.Ok(response);
    }).WithName("SNMPWALK")
      .Produces<SNMPWalkResponse>(StatusCodes.Status200OK)
      .ProducesProblem(StatusCodes.Status400BadRequest)
      .WithSummary("SNMP Walk")
      .WithDescription("SNMP Walk");
  }
}

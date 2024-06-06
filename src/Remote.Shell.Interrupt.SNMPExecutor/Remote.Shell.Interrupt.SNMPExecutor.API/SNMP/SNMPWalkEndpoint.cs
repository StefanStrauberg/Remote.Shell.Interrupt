namespace Remote.Shell.Interrupt.SNMPExecutor.API.SNMP;

public record SNMPWalkRequest(string Host,
                              string Community,
                              string OID);

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

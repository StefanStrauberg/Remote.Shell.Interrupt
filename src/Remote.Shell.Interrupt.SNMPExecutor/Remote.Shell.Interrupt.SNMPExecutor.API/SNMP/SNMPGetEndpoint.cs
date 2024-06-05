namespace Remote.Shell.Interrupt.SNMP.API.Executor;

public record SNMPGetRequest(string Host,
                             string Community,
                             string OID);

public record SNMPGetResponse(string OID,
                              string TypeCode,
                              string Data);

public class SNMPGetEndpoint : ICarterModule
{
  public void AddRoutes(IEndpointRouteBuilder app)
  {
    app.MapGet("/executor/get", async (SNMPGetRequest request, ISender sender) =>
    {
      var command = request.Adapt<SNMPGetCommand>();
      var result = await sender.Send(command);
      var response = result.Adapt<SNMPGetResponse>();
      return Results.Ok(response);
    }).WithName("SNMPGET")
      .Produces<SNMPGetResponse>(StatusCodes.Status200OK)
      .ProducesProblem(StatusCodes.Status400BadRequest)
      .WithSummary("SNMP GET")
      .WithDescription("SNMP GET");
  }
}

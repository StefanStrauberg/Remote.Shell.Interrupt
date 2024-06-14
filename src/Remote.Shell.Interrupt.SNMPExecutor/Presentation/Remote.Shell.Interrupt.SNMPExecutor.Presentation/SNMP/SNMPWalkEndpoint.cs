namespace Remote.Shell.Interrupt.SNMPExecutor.Presentation.SNMP;

public class SNMPWalkEndpoint : ICarterModule
{
  public void AddRoutes(IEndpointRouteBuilder app)
  {
    app.MapGet("/walk", async ([FromBody] SNMPGetWalkRequest request, ISender sender) =>
    {
      var command = request.Adapt<SNMPWalkCommand>();
      var result = await sender.Send(command);
      return Results.Ok(result.ToString());
    }).WithName("SNMPWALK")
      .Produces<JsonArray>(StatusCodes.Status200OK)
      .ProducesProblem(StatusCodes.Status400BadRequest)
      .WithSummary("SNMP Walk")
      .WithDescription("SNMP Walk");
  }
}

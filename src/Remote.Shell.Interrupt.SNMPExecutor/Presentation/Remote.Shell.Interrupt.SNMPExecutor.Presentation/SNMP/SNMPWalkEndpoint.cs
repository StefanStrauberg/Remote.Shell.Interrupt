namespace Remote.Shell.Interrupt.SNMPExecutor.Presentation.SNMP;

public class SNMPWalkEndpoint : ICarterModule
{
  public void AddRoutes(IEndpointRouteBuilder app)
  {
    app.MapGet("/walk", async (SNMPGetWalkRequest request, ISender sender) =>
    {
      var command = request.Adapt<SNMPWalkCommand>();
      var result = await sender.Send(command);
      var response = result.Adapt<SNMPGetWalkResponse>();
      return Results.Ok(response);
    }).WithName("SNMPWALK")
      .Produces<SNMPGetWalkResponse>(StatusCodes.Status200OK)
      .ProducesProblem(StatusCodes.Status400BadRequest)
      .WithSummary("SNMP Walk")
      .WithDescription("SNMP Walk");
  }
}

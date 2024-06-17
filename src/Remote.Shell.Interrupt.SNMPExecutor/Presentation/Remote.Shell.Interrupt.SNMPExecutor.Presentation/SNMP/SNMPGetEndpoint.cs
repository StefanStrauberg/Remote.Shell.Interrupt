namespace Remote.Shell.Interrupt.SNMPExecutor.Presentation.SNMP;

public class SNMPGetEndpoint : ICarterModule
{
  public void AddRoutes(IEndpointRouteBuilder app)
  {
    app.MapGet("/get", async ([FromBody] SNMPGetWalkRequest request, ISender sender) =>
    {
      var command = request.Adapt<SNMPGetCommand>();
      var result = await sender.Send(command);
      return Results.Ok(result);
    }).WithName("SNMPGet")
      .Produces<Information>(StatusCodes.Status200OK)
      .ProducesProblem(StatusCodes.Status400BadRequest)
      .WithSummary("SNMP Get")
      .WithDescription("SNMP Get");
  }
}

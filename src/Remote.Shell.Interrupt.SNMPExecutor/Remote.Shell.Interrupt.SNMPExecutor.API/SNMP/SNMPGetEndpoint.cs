namespace Remote.Shell.Interrupt.SNMP.API.Executor;

public class SNMPGetEndpoint : ICarterModule
{
  public void AddRoutes(IEndpointRouteBuilder app)
  {
    app.MapGet("/executor/get", async (SNMPGetWalkRequest request, ISender sender) =>
    {
      var command = request.Adapt<SNMPGetCommand>();
      var result = await sender.Send(command);
      var response = result.Adapt<SNMPGetWalkResponse>();
      return Results.Ok(response);
    }).WithName("SNMPGet")
      .Produces<SNMPGetWalkResponse>(StatusCodes.Status200OK)
      .ProducesProblem(StatusCodes.Status400BadRequest)
      .WithSummary("SNMP Get")
      .WithDescription("SNMP Get");
  }
}

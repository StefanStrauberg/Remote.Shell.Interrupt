namespace Remote.Shell.Interrupt.Presentation.Assignments;

public class AssignmentGetEndpoint : ICarterModule
{
  public void AddRoutes(IEndpointRouteBuilder app)
  {
    app.MapGet("assignment/get", async ([FromBody] CreateAssignmentCommand request, ISender sender) =>
    {
      var result = await sender.Send(request);
    }).WithName("SNMPGet")
      .ProducesProblem(StatusCodes.Status400BadRequest)
      .WithSummary("SNMP Get")
      .WithDescription("SNMP Get");
  }
}

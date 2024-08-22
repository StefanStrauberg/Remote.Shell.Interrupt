namespace Remote.Shell.Interrupt.Presentation.Assignments;

public class GetAssignmentsEndpoint : ICarterModule
{
  public void AddRoutes(IEndpointRouteBuilder app)
  {
    app.MapGet("assignments", async ([FromBody] GetAssignmentsQuery request, ISender sender) =>
    {
      var result = await sender.Send(request);
    }).WithName("GetAssignments")
      .ProducesProblem(StatusCodes.Status400BadRequest)
      .WithSummary("Get Assignments")
      .WithDescription("Get all assignments");
  }
}

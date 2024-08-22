namespace Remote.Shell.Interrupt.Presentation.Assignments;

public class CreateAssignmentsEndpoint : ICarterModule
{
  public void AddRoutes(IEndpointRouteBuilder app)
  {
    app.MapPost("assignments", async ([FromBody] CreateAssignmentCommand request, ISender sender) =>
    {
      var result = await sender.Send(request);
    }).WithName("CreateAssignment")
      .ProducesProblem(StatusCodes.Status400BadRequest)
      .WithSummary("Create Assignment")
      .WithDescription("Creates a new assignment.");
  }
}
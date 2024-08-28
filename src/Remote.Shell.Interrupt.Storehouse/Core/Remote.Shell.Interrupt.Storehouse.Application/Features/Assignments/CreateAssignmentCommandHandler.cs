namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Assignments;

public record CreateAssignmentCommand(CreateAssignmentDTO CreateAssignmentDTO) : ICommand;

internal class CreateAssignmentCommandHandler(IAssignmentRepository assignmentRepository)
  : ICommandHandler<CreateAssignmentCommand, Unit>
{
  readonly IAssignmentRepository _assignmentRepository = assignmentRepository
    ?? throw new ArgumentNullException(nameof(assignmentRepository));

  async Task<Unit> IRequestHandler<CreateAssignmentCommand, Unit>.Handle(CreateAssignmentCommand request,
                                                                         CancellationToken cancellationToken)
  {
    // The name should be unique
    Expression<Func<Assignment, bool>> filter = x => x.Name == request.CreateAssignmentDTO
                                                                      .Name;
    // Check if an adding assignment is existing by name
    var existingAssignment = await _assignmentRepository.ExistsAsync(filterExpression: filter,
                                                                     cancellationToken: cancellationToken);
    // If an adding assignment exists
    // throw EntityAlreadyExists exception
    if (existingAssignment)
    {
      var errorMessage = new ExpressionToStringConverter<Assignment>().Convert(filter);
      throw new EntityAlreadyExists(errorMessage);
    }

    // convert an inputing dto to domain model 
    var assignment = request.CreateAssignmentDTO
                            .Adapt<Assignment>();

    assignment.Created = DateTime.Now;
    assignment.Modified = DateTime.Now;

    await _assignmentRepository.InsertOneAsync(document: assignment,
                                               cancellationToken: cancellationToken);

    return Unit.Value;
  }
}

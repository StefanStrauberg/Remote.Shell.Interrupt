namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Assignments;

public class CreateAssignmentDTO
{
  public string Name { get; set; } = string.Empty;
  public TypeOfRequest TypeOfRequest { get; set; }
  public string TargetFieldName { get; set; } = string.Empty;
  public string OID { get; set; } = string.Empty;
};

public class CreateAssignmentDTOValidator : AbstractValidator<CreateAssignmentDTO>
{
  public CreateAssignmentDTOValidator()
  {
    RuleFor(x => x.Name).NotNull().WithMessage("Name can't be null")
                        .NotEmpty().WithMessage("Name can't be empty");

    RuleFor(x => x.TypeOfRequest).NotNull().WithMessage("Invalid TypeOfRequest value")
                                 .NotEqual(default(TypeOfRequest)).WithMessage("TypeOfRequest is required");

    RuleFor(x => x.OID).NotNull().WithMessage("OID can't be null")
                       .NotEmpty().WithMessage("OID can't be empty")
                       .WithMessage("OID is required")
                       .Matches(@"^\d+(\.\d+)+$")
                       .WithMessage("Invalid OID. OID should be mtches x.x.x where x are numbers");
  }
}

public record CreateAssignmentCommand(CreateAssignmentDTO CreateAssignmentDTO) : ICommand;

internal class CreateAssignmentCommandHandler(IAssignmentRepository assignmentRepository)
  : ICommandHandler<CreateAssignmentCommand, Unit>
{
  readonly IAssignmentRepository _assignmentRepository = assignmentRepository
    ?? throw new ArgumentNullException(nameof(assignmentRepository));

  async Task<Unit> IRequestHandler<CreateAssignmentCommand, Unit>.Handle(CreateAssignmentCommand request,
                                                                         CancellationToken cancellationToken)
  {
    Expression<Func<Assignment, bool>> filter = x => x.Name == request.CreateAssignmentDTO.Name;

    var existingAssignment = await _assignmentRepository.ExistsAsync(filterExpression: filter,
                                                                     cancellationToken: cancellationToken);

    if (existingAssignment)
    {
      var errorMessage = new ExpressionToStringConverter<Assignment>().Convert(filter);
      throw new EntityAlreadyExists(errorMessage);
    }

    var assignment = request.CreateAssignmentDTO
                            .Adapt<Assignment>();

    await _assignmentRepository.InsertOneAsync(document: assignment,
                                               cancellationToken: cancellationToken);

    return Unit.Value;
  }
}

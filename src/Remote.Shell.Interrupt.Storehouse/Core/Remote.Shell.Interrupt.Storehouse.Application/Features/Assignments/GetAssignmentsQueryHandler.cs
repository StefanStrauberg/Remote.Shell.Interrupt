namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Assignments;

public record GetAssignmentsQuery() : IQuery<IEnumerable<AssignmentDTO>>;

internal class GetAssignmentsQueryHandler(IAssignmentRepository assignmentRepository)
  : IQueryHandler<GetAssignmentsQuery, IEnumerable<AssignmentDTO>>
{
  readonly IAssignmentRepository _assignmentRepository = assignmentRepository
    ?? throw new ArgumentNullException(nameof(assignmentRepository));

  async Task<IEnumerable<AssignmentDTO>> IRequestHandler<GetAssignmentsQuery, IEnumerable<AssignmentDTO>>.Handle(GetAssignmentsQuery request,
                                                                                                                 CancellationToken cancellationToken)
  {
    var assignments = await _assignmentRepository.GetAllAsync(cancellationToken);
    var assignmentsDTOs = assignments.Adapt<IEnumerable<AssignmentDTO>>();

    return assignmentsDTOs;
  }
}

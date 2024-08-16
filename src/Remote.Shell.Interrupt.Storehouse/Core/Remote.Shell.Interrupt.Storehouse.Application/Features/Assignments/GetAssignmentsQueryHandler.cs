namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Assignments;

public record GetAssignmentsQuery() : IQuery<IEnumerable<Assignment>>;

internal class GetAssignmentsQueryHandler(IAssignmentRepository assignmentRepository)
  : IQueryHandler<GetAssignmentsQuery, IEnumerable<Assignment>>
{
  readonly IAssignmentRepository _assignmentRepository = assignmentRepository
    ?? throw new ArgumentNullException(nameof(assignmentRepository));

  async Task<IEnumerable<Assignment>> IRequestHandler<GetAssignmentsQuery, IEnumerable<Assignment>>.Handle(GetAssignmentsQuery request,
                                                                                                           CancellationToken cancellationToken)
  {
    var assignments = await _assignmentRepository.GetAllAsync(cancellationToken);

    return assignments;
  }
}

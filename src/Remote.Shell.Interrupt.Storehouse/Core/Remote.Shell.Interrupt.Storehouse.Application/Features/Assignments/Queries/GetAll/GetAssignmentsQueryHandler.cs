namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Assignments.Queries.GetAll;

public record GetAssignmentsQuery() : IQuery<IEnumerable<AssignmentDTO>>;

internal class GetAssignmentsQueryHandler(IAssignmentRepository assignmentRepository,
                                          IMapper mapper)
  : IQueryHandler<GetAssignmentsQuery, IEnumerable<AssignmentDTO>>
{
  readonly IAssignmentRepository _assignmentRepository = assignmentRepository
    ?? throw new ArgumentNullException(nameof(assignmentRepository));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<IEnumerable<AssignmentDTO>> IRequestHandler<GetAssignmentsQuery, IEnumerable<AssignmentDTO>>.Handle(GetAssignmentsQuery request,
                                                                                                                 CancellationToken cancellationToken)
  {
    var assignments = await _assignmentRepository.GetAllAsync(cancellationToken);
    var assignmentsDTOs = _mapper.Map<IEnumerable<AssignmentDTO>>(assignments);

    return assignmentsDTOs;
  }
}

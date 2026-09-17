using Mediator;

namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Workflows.Commands.CreateWorkflow;

/// <summary>
/// Command for creating a new <see cref="WorkflowDefinition"/> entity from a
/// <see cref="CreateWorkflowDTO"/> payload. Returns the new entity's ID directly, unlike
/// <see cref="Application.Features.Core.Commands.CreateEntityCommand{TCreateDto}"/> (the shared
/// base every other "create" command uses, which returns <see cref="Unit"/>) - the SPA's
/// workflow designer needs the ID immediately after a successful create to navigate straight
/// into the editor for the graph it just saved, and re-deriving it with a follow-up
/// GetWorkflowsByFilter-by-name lookup is ambiguous under a concurrent create/rename and is an
/// extra round trip besides.
/// </summary>
public record CreateWorkflowCommand(CreateWorkflowDTO WorkflowDto) : CQRS.ICommand<Guid>;

/// <summary>
/// Handles creation of <see cref="WorkflowDefinition"/> entities. Mirrors
/// <c>CreateEntityCommandHandler</c>'s duplicate-check-then-persist shape (kept independent
/// rather than sharing that base, since this command's <see cref="Guid"/> return type differs
/// from every other entity's <see cref="Unit"/>), and leverages the same specification-based
/// duplicate prevention. The unique index on <see cref="WorkflowDefinition.Name"/>
/// (see <c>WorkflowDefinitionConfiguration</c>) is still the actual race-safe guarantee;
/// this check only turns a would-be constraint violation into a friendlier 409-style error
/// for the common case.
/// </summary>
internal class CreateWorkflowCommandHandler(IWorkflowUnitOfWork workflowUnitOfWork,
                                             IWorkflowSpecification specification,
                                             IQueryFilterParser queryFilterParser,
                                             IMapper mapper)
  : CQRS.ICommandHandler<CreateWorkflowCommand, Guid>
{
  public async ValueTask<Guid> Handle(CreateWorkflowCommand request, CancellationToken cancellationToken)
  {
    var filter = queryFilterParser.ParseFilters<WorkflowDefinition>(new RequestParameters
    {
      Filters =
        [
          new()
          {
            PropertyPath = nameof(WorkflowDefinition.Name),
            Operator = FilterOperator.Equals,
            Value = request.WorkflowDto.Name
          }
        ]
    }.Filters);
    var spec = specification.Clone();

    if (filter is not null)
      spec.AddFilter(filter);

    bool exists = await workflowUnitOfWork.Workflows.AnyByQueryAsync(spec, cancellationToken);

    if (exists is true)
      throw new EntityAlreadyExists(typeof(WorkflowDefinition), spec.Criterias?.ToString() ?? nameof(BaseEntity.Id));

    var workflow = mapper.Map<WorkflowDefinition>(request.WorkflowDto);

    workflowUnitOfWork.StartTransaction();
    workflowUnitOfWork.Workflows.InsertOne(workflow);
    workflowUnitOfWork.Complete();

    // Id is DB-generated (see WorkflowDefinitionConfiguration's gen_random_uuid() default) -
    // populated back onto this same tracked entity instance by SaveChanges(), not assigned here.
    return workflow.Id;
  }
}

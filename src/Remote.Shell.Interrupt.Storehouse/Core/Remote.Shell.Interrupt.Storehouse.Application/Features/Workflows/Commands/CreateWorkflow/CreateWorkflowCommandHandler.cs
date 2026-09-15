namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Workflows.Commands.CreateWorkflow;

/// <summary>
/// Command for creating a new <see cref="WorkflowDefinition"/> entity using a <see cref="CreateWorkflowDTO"/> payload.
/// </summary>
public record CreateWorkflowCommand(CreateWorkflowDTO WorkflowDto)
  : CreateEntityCommand<CreateWorkflowDTO>(WorkflowDto);

/// <summary>
/// Handles creation of <see cref="WorkflowDefinition"/> entities. Leverages specifications
/// to prevent duplicates based on Name.
/// </summary>
internal class CreateWorkflowCommandHandler(IWorkflowUnitOfWork workflowUnitOfWork,
                                            IWorkflowSpecification specification,
                                            IQueryFilterParser queryFilterParser,
                                            IMapper mapper)
  : CreateEntityCommandHandler<WorkflowDefinition, CreateWorkflowDTO, CreateWorkflowCommand>(specification, mapper)
{
  protected override Expression<Func<WorkflowDefinition, bool>>? BuildDuplicateCheckFilter(CreateWorkflowDTO createDto)
    => queryFilterParser.ParseFilters<WorkflowDefinition>(new RequestParameters
    {
      Filters =
        [
          new()
          {
            PropertyPath = nameof(WorkflowDefinition.Name),
            Operator = FilterOperator.Equals,
            Value = createDto.Name
          }
        ]
    }.Filters);

  protected override async Task ValidateEntityDoesNotExistAsync(ISpecification<WorkflowDefinition> specification,
                                                                 CancellationToken cancellationToken)
  {
    bool exists = await workflowUnitOfWork.Workflows.AnyByQueryAsync(specification, cancellationToken);

    if (exists is true)
      throw new EntityAlreadyExists(typeof(WorkflowDefinition), specification.Criterias?.ToString() ?? nameof(BaseEntity.Id));
  }

  protected override void PersistNewEntity(WorkflowDefinition workflow)
  {
    workflowUnitOfWork.StartTransaction();
    workflowUnitOfWork.Workflows.InsertOne(workflow);
    workflowUnitOfWork.Complete();
  }
}

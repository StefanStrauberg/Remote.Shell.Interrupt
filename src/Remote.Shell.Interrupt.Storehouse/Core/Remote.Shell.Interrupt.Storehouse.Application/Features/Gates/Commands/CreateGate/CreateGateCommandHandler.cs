namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Commands.CreateGate;

/// <summary>
/// Command for creating a new <see cref="Gate"/> entity using a <see cref="CreateGateDTO"/> payload.
/// </summary>
public record CreateGateCommand(CreateGateDTO GateDto)
  : CreateEntityCommand<CreateGateDTO>(GateDto);

/// <summary>
/// Handles creation of <see cref="Gate"/> entities using validation, filtering, and persistence logic.
/// Leverages specifications to prevent duplicates based on IP address.
/// </summary>
internal class CreateGateCommandHandler(IGateUnitOfWork gateUnitOfWork,
                                        IGateSpecification specification,
                                        IQueryFilterParser queryFilterParser,
                                        IMapper mapper)
  : CreateEntityCommandHandler<Gate, CreateGateDTO, CreateGateCommand>(specification, mapper)
{
  /// <summary>
  /// Constructs a filter expression to identify existing gates with a matching IP address.
  /// </summary>
  /// <param name="createDto">The DTO containing gate creation data.</param>
  /// <returns>A LINQ expression for uniqueness filtering.</returns>
  protected override Expression<Func<Gate, bool>>? BuildDuplicateCheckFilter(CreateGateDTO createDto)
    => queryFilterParser.ParseFilters<Gate>(new RequestParameters
    {
      Filters =
        [
          new()
          {
              PropertyPath = "IPAddress",
              Operator = FilterOperator.Equals,
              Value = createDto.IPAddress
          }
        ]
    }.Filters);

  /// <summary>
  /// Ensures that no gate entity already exists matching the specified filter.
  /// Throws <see cref="EntityAlreadyExists"/> if a duplicate is detected.
  /// </summary>
  /// <param name="specification">The specification used to check for duplicates.</param>
  /// <param name="cancellationToken">Token to cancel the operation if needed.</param>
  protected override async Task ValidateEntityDoesNotExistAsync(ISpecification<Gate> specification,
                                                                CancellationToken cancellationToken)
  {
    bool exists = await gateUnitOfWork.Gates.AnyByQueryAsync(specification, cancellationToken);

    if (exists is true)
      throw new EntityAlreadyExists(typeof(Gate), specification.ToString() ?? string.Empty);
  }

  /// <summary>
  /// Persists the newly mapped gate entity to the underlying data store.
  /// </summary>
  /// <param name="gate">The gate entity to insert.</param>
  protected override void PersistNewEntity(Gate gate)
  {
    gateUnitOfWork.Gates.InsertOne(gate);
    gateUnitOfWork.Complete();
  }
}

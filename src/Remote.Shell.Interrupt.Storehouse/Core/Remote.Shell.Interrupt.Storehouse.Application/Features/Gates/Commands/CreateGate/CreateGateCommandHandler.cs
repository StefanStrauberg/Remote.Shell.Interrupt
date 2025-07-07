namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Commands.CreateGate;

/// <summary>
/// Defines a command for creating a new gate entity based on supplied DTO.
/// </summary>
/// <param name="GateDto">Contains the input data for the gate creation process.</param>
public record CreateGateCommand(CreateGateDTO GateDto) : ICommand<Unit>;

/// <summary>
/// Handles <see cref="CreateGateCommand"/> by validating uniqueness,
/// converting input to entity, and persisting the new gate.
/// </summary>
/// <param name="gateUnitOfWork">Provides transactional access to gate persistence operations.</param>
/// <param name="specification">Base specification used to build uniqueness checks.</param>
/// <param name="queryFilterParser">Converts filter expressions into executable predicates.</param>
/// <param name="mapper">Maps data transfer objects to domain entities.</param>
internal class CreateGateCommandHandler(IGateUnitOfWork gateUnitOfWork,
                                        IGateSpecification specification,
                                        IQueryFilterParser queryFilterParser,
                                        IMapper mapper)
  : ICommandHandler<CreateGateCommand, Unit>
{
  /// <summary>
  /// Executes the create gate workflow—checks for duplication, transforms DTO, and persists.
  /// </summary>
  /// <param name="request">Contains the gate creation input.</param>
  /// <param name="cancellationToken">Propagates cancellation signals from upstream.</param>
  /// <returns><see cref="Unit.Value"/> upon successful completion.</returns>
  public async Task<Unit> Handle(CreateGateCommand request,
                                 CancellationToken cancellationToken)
  {
    var ipAddressFilter = BuildIpAddressFilter(request.GateDto.IPAddress);
    var specification = BuildSpecification(ipAddressFilter);

    await EnsureGateDoesNotExistAsync(specification, cancellationToken);

    var newGate = MapToEntity(request.GateDto);
    PersistNewGate(newGate);

    return Unit.Value;
  }

  /// <summary>
  /// Constructs a predicate expression using IP address.
  /// </summary>
  /// <param name="ipAddress">The IP address to filter by.</param>
  /// <returns>Expression representing the IP-based filter condition.</returns>
  Expression<Func<Gate, bool>>? BuildIpAddressFilter(string ipAddress)
    => queryFilterParser.ParseFilters<Gate>(new RequestParameters
    {
      Filters =
        [
          new()
          {
              PropertyPath = "IPAddress",
              Operator = FilterOperator.Equals,
              Value = ipAddress
          }
        ]
    }.Filters);

  /// <summary>
  /// Builds a specification object that includes optional filter logic.
  /// </summary>
  /// <param name="filterExpr">An expression predicate for entity filtering.</param>
  /// <returns>A specification configured with the provided filter.</returns>
  ISpecification<Gate> BuildSpecification(Expression<Func<Gate, bool>>? filterExpr)
  {
    var spec = specification.Clone();

    if (filterExpr is not null)
      spec.AddFilter(filterExpr);

    return spec;
  }

  /// <summary>
  /// Validates that no existing gate matches the provided specification.
  /// </summary>
  /// <param name="specification">Specification used for gate uniqueness check.</param>
  /// <param name="cancellationToken">Supports cancellation of the async operation.</param>
  /// <exception cref="EntityAlreadyExists">Thrown if a gate with the same filter already exists.</exception>
  async Task EnsureGateDoesNotExistAsync(ISpecification<Gate> specification,
                                         CancellationToken cancellationToken)
  {
    bool exists = await gateUnitOfWork.Gates.AnyByQueryAsync(specification, cancellationToken);

    if (exists is true)
      throw new EntityAlreadyExists(typeof(Gate), specification.ToString() ?? string.Empty);
  }

  /// <summary>
  /// Maps a gate creation DTO to its domain entity counterpart.
  /// </summary>
  /// <param name="gateDto">DTO carrying gate input data.</param>
  /// <returns>A <see cref="Gate"/> entity ready for persistence.</returns>
  Gate MapToEntity(CreateGateDTO gateDto)
    => mapper.Map<Gate>(gateDto);

  /// <summary>
  /// Persists a newly created gate to the database and completes the transaction.
  /// </summary>
  /// <param name="gate">The new gate entity to store.</param>
  void PersistNewGate(Gate gate)
  {
    gateUnitOfWork.Gates.InsertOne(gate);
    gateUnitOfWork.Complete();
  }
}

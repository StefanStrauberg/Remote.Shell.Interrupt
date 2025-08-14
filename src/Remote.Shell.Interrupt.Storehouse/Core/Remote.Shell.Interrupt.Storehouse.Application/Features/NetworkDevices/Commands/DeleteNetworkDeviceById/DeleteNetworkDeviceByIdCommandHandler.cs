namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Commands.DeleteNetworkDeviceById;

/// <summary>
/// Command to delete a <see cref="NetworkDevice"/> entity by its unique identifier.
/// </summary>
public record DeleteNetworkDeviceByIdCommand(Guid Id) : DeleteEntityCommand(Id);

/// <summary>
/// Handler for processing <see cref="DeleteNetworkDeviceByIdCommand"/> requests.
/// Implements network device–specific logic for existence validation, retrieval, and deletion.
/// </summary>
/// <param name="netDevUnitOfWork">Unit of work for network device operations.</param>
/// <param name="specification">Base specification used for querying network devices.</param>
/// <param name="queryFilterParser">Parser for translating request filters into query expressions.</param>
internal class DeleteNetworkDeviceByIdCommandHandler(INetDevUnitOfWork netDevUnitOfWork,
                                                     INetworkDeviceSpecification specification,
                                                     IQueryFilterParser queryFilterParser)
  : DeleteEntityCommandHandler<NetworkDevice, DeleteNetworkDeviceByIdCommand>(specification, queryFilterParser)
{
  /// <summary>
  /// Deletes the specified network device along with its child entities.
  /// </summary>
  /// <param name="entity">The network device to delete.</param>
  protected override void DeleteEntity(NetworkDevice entity)
    => netDevUnitOfWork.NetworkDevices.DeleteOneWithChilren(entity);

  /// <summary>
  /// Validates that the network device exists based on the specification.
  /// </summary>
  /// <param name="specification">Specification used to locate the device.</param>
  /// <param name="cancellationToken">Token for cancelling the operation.</param>
  /// <exception cref="EntityNotFoundException">
  /// Thrown when no network device matches the provided specification.
  /// </exception>
  protected override async Task EnsureEntityExistAsync(ISpecification<NetworkDevice> specification,
                                                       CancellationToken cancellationToken)
  {
    bool exists = await netDevUnitOfWork.NetworkDevices.AnyByQueryAsync(specification, cancellationToken);

    if (exists is not true)
      throw new EntityNotFoundException(typeof(NetworkDevice), specification.ToString() ?? string.Empty);
  }

  /// <summary>
  /// Fetches the network device along with its child entities.
  /// </summary>
  /// <param name="specification">Specification used to locate the device.</param>
  /// <param name="cancellationToken">Token for cancelling the operation.</param>
  /// <returns>The fetched <see cref="NetworkDevice"/> entity.</returns>
  protected override async Task<NetworkDevice> FetchEntityAsync(ISpecification<NetworkDevice> specification,
                                                                CancellationToken cancellationToken)
    => await netDevUnitOfWork.NetworkDevices.GetOneWithChildrenAsync(specification, cancellationToken);
}

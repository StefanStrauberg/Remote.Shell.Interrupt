namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Commands.DeleteNetworkDeviceById;

/// <summary>
/// Represents a command to delete a network device by its unique identifier.
/// </summary>
/// <param name="Id">The unique identifier of the network device to delete.</param>
public record DeleteNetworkDeviceByIdCommand(Guid Id) : ICommand;

/// <summary>
/// Handles the DeleteNetworkDeviceByIdCommand and removes the specified network device.
/// </summary>
/// <remarks>
/// This handler verifies the existence of the network device, retrieves it, 
/// and deletes it from the repository. If the device is not found, an exception is thrown.
/// </remarks>
/// <param name="netDevUnitOfWork">Unit of work for network device-related database operations.</param>
/// <param name="specification">Specification used for filtering network devices.</param>
/// <param name="queryFilterParser">Parser for processing filter expressions.</param>
internal class DeleteNetworkDeviceByIdCommandHandler(INetDevUnitOfWork netDevUnitOfWork,
                                                     INetworkDeviceSpecification specification,
                                                     IQueryFilterParser queryFilterParser)
  : ICommandHandler<DeleteNetworkDeviceByIdCommand, Unit>
{
  /// <summary>
  /// Handles the request to delete a network device by its ID.
  /// </summary>
  /// <param name="request">The command containing the network device ID to delete.</param>
  /// <param name="cancellationToken">Token to handle request cancellation.</param>
  /// <returns>A unit value indicating successful execution.</returns>
  async Task<Unit> IRequestHandler<DeleteNetworkDeviceByIdCommand, Unit>.Handle(DeleteNetworkDeviceByIdCommand request,
                                                                                CancellationToken cancellationToken)
  {
    // Create filtering parameters based on the provided network device ID
    var requestParameters = new RequestParameters
    {
      Filters = [
        new ()
        {
          PropertyPath = "Id",
          Operator = FilterOperator.Equals,
          Value = request.Id.ToString()
        }
      ]
    };

    // Parse filter expression
    var filterExpr = queryFilterParser.ParseFilters<NetworkDevice>(requestParameters.Filters);

    // Build base specification with filtering applied
    var baseSpec = BuildSpecification(specification,
                                      filterExpr);

    // Check if the network device exists
    var existing = await netDevUnitOfWork.NetworkDevices
                                         .AnyByQueryAsync(baseSpec,
                                                          cancellationToken);
    
    // If the network device is not found, throw an exception
    if (!existing)
      throw new EntityNotFoundException(typeof(Gate),
                                        filterExpr is not null ? filterExpr.ToString() : string.Empty);

    // Retrieve the network device for deletion
    var networkDeviceToDelete = await netDevUnitOfWork.NetworkDevices
                                                      .GetOneWithChildrenAsync(baseSpec,
                                                                                cancellationToken);

    // Delete the found device from the repository
    netDevUnitOfWork.NetworkDevices
                    .DeleteOneWithChilren(networkDeviceToDelete);

    // Commit the transaction
    netDevUnitOfWork.Complete();

    // Return successful execution result
    return Unit.Value;
  }

  /// <summary>
  /// Builds the specification by applying filtering criteria.
  /// </summary>
  /// <param name="baseSpec">The base network device specification.</param>
  /// <param name="filterExpr">The filter expression to apply.</param>
  /// <returns>An updated specification with filtering applied.</returns>
  static INetworkDeviceSpecification BuildSpecification(INetworkDeviceSpecification baseSpec,
                                               Expression<Func<NetworkDevice, bool>>? filterExpr)
  {
    var spec = baseSpec;

    if (filterExpr is not null)
        spec.AddFilter(filterExpr);

    return spec;
  }
}

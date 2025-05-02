namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Commands.DeleteNetworkDevices;

/// <summary>
/// Represents a command to delete all network devices.
/// </summary>
public record DeleteNetworkDevicesCommand : ICommand<Unit>;

/// <summary>
/// Handles the DeleteNetworkDevicesCommand and removes all network devices.
/// </summary>
/// <remarks>
/// This handler retrieves all network devices, iterates through them, 
/// and executes individual deletion commands for each device.
/// </remarks>
/// <param name="netDevUnitOfWork">Unit of work for network device-related operations.</param>
/// <param name="specification">Specification used for filtering network devices.</param>
/// <param name="queryFilterParser">Parser for processing filter expressions.</param>
internal class DeleteNetworkDevicesCommandHandler(INetDevUnitOfWork netDevUnitOfWork,
                                                  INetworkDeviceSpecification specification,
                                                  IQueryFilterParser queryFilterParser)
  : ICommandHandler<DeleteNetworkDevicesCommand, Unit>
{
  /// <summary>
  /// Handles the request to delete all network devices.
  /// </summary>
  /// <param name="request">The command initiating the deletion process.</param>
  /// <param name="cancellationToken">Token to support request cancellation.</param>
  /// <returns>A unit value indicating successful execution.</returns>
  async Task<Unit> IRequestHandler<DeleteNetworkDevicesCommand, Unit>.Handle(DeleteNetworkDevicesCommand request,
                                                                             CancellationToken cancellationToken)
  {
    // Retrieve all network devices
    var networkDevices = await netDevUnitOfWork.NetworkDevices
                                               .GetAllAsync(cancellationToken);

    // Instantiate command handler for deleting a single device
    var deleteNetworkDeviceByIdCommandHandler = new DeleteNetworkDeviceByIdCommandHandler(netDevUnitOfWork,
                                                                                          specification,
                                                                                          queryFilterParser);

    // Iterate through network devices and delete each one
    foreach (var item in networkDevices)
    {
      var deleteNetworkDeviceByIdCommand = new DeleteNetworkDeviceByIdCommand(item.Id);

      await ((IRequestHandler<DeleteNetworkDeviceByIdCommand, Unit>)deleteNetworkDeviceByIdCommandHandler).Handle(deleteNetworkDeviceByIdCommand,
                                                                                                                  cancellationToken);
    }

    // Return successful execution result
    return Unit.Value;
  }
}

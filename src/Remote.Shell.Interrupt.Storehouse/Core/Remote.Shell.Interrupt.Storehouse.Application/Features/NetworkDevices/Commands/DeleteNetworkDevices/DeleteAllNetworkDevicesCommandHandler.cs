using Mediator;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Identity;

namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Commands.DeleteNetworkDevices;

/// <summary>
/// Represents a command to delete all network devices.
/// </summary>
public record DeleteAllNetworkDevicesCommand : CQRS.ICommand<Unit>;

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
/// <param name="currentUserService">Identifies the caller for the audit trail of this destructive operation.</param>
/// <param name="logger">Records who wiped the device inventory and how many devices were removed.</param>
internal class DeleteAllNetworkDevicesCommandHandler(INetDevUnitOfWork netDevUnitOfWork,
                                                     INetworkDeviceSpecification specification,
                                                     IQueryFilterParser queryFilterParser,
                                                     ICurrentUserService currentUserService,
                                                     IAppLogger<DeleteAllNetworkDevicesCommandHandler> logger)
  : CQRS.ICommandHandler<DeleteAllNetworkDevicesCommand, Unit>
{
  /// <summary>
  /// Handles the request to delete all network devices.
  /// </summary>
  /// <param name="request">The command initiating the deletion process.</param>
  /// <param name="cancellationToken">Token to support request cancellation.</param>
  /// <returns>A unit value indicating successful execution.</returns>
  async ValueTask<Unit> IRequestHandler<DeleteAllNetworkDevicesCommand, Unit>.Handle(DeleteAllNetworkDevicesCommand request,
                                                                             CancellationToken cancellationToken)
  {
    // Retrieve all network devices
    var networkDevices = (await netDevUnitOfWork.NetworkDevices
                                                .GetAllAsync(cancellationToken)).ToList();

    logger.LogWarning("User {UserId} ({Email}) is deleting all {DeviceCount} network device(s).",
                      currentUserService.UserId?.ToString() ?? "unknown",
                      currentUserService.Email ?? "unknown",
                      networkDevices.Count);

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

    logger.LogWarning("User {UserId} ({Email}) deleted all {DeviceCount} network device(s).",
                      currentUserService.UserId?.ToString() ?? "unknown",
                      currentUserService.Email ?? "unknown",
                      networkDevices.Count);

    // Return successful execution result
    return Unit.Value;
  }
}

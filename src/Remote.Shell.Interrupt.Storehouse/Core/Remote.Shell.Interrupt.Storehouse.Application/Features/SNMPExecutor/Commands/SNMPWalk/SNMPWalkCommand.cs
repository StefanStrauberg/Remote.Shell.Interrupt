namespace Remote.Shell.Interrupt.Storehouse.Application.Features.SNMPExecutor.Commands.SNMPWalk;

/// <summary>
/// Represents a command to execute an SNMP Walk request.
/// </summary>
/// <param name="Host">The target host for the SNMP request.</param>
/// <param name="Community">The SNMP community string used for authentication.</param>
/// <param name="OID">The object identifier (OID) specifying the starting point for the walk operation.</param>
public record class SNMPWalkCommand(string Host,
                                    string Community,
                                    string OID) : IRequest<IEnumerable<SNMPResponse>>;

/// <summary>
/// Handles the SNMPWalkCommand and executes an SNMP Walk operation.
/// </summary>
/// <remarks>
/// This handler sends an SNMP Walk request using the provided host, community string, and starting OID.
/// The response includes multiple SNMP objects within the given OID subtree.
/// </remarks>
/// <param name="executor">The SNMP command executor responsible for performing the request.</param>
internal class SNMPWalkCommandHandler(ISNMPCommandExecutor executor) : IRequestHandler<SNMPWalkCommand, IEnumerable<SNMPResponse>>
{
  /// <summary>
  /// Handles the SNMP Walk request and retrieves multiple SNMP responses from the target device.
  /// </summary>
  /// <param name="request">The SNMP Walk command containing host, community, and starting OID information.</param>
  /// <param name="cancellationToken">Token to handle request cancellation.</param>
  /// <returns>A collection of SNMP responses containing multiple objects from the OID subtree.</returns>
  async Task<IEnumerable<SNMPResponse>> IRequestHandler<SNMPWalkCommand, IEnumerable<SNMPResponse>>.Handle(SNMPWalkCommand request,
                                                                                                           CancellationToken cancellationToken)
    => await executor.WalkCommand(request.Host,
                                  request.Community,
                                  request.OID,
                                  cancellationToken);
}

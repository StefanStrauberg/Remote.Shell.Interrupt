namespace Remote.Shell.Interrupt.Storehouse.Application.Features.SNMPExecutor.Commands.SNMPGet;

/// <summary>
/// Represents a command to execute an SNMP GET request.
/// </summary>
/// <param name="Host">The target host for the SNMP request.</param>
/// <param name="Community">The SNMP community string used for authentication.</param>
/// <param name="OID">The object identifier (OID) specifying the data to retrieve.</param>
public record SNMPGetCommand(string Host,
                             string Community,
                             string OID) : IRequest<SNMPResponse>;

/// <summary>
/// Handles the SNMPGetCommand and executes the SNMP GET operation.
/// </summary>
/// <remarks>
/// This handler uses an SNMP command executor to send a GET request and retrieve the response.
/// </remarks>
/// <param name="executor">The SNMP command executor responsible for performing the request.</param>
internal class SNMPGetCommandHandler(ISNMPCommandExecutor executor) : IRequestHandler<SNMPGetCommand, SNMPResponse>
{

  /// <summary>
  /// Handles the SNMP GET request and retrieves the response from the target device.
  /// </summary>
  /// <param name="request">The SNMP GET command containing host, community, and OID information.</param>
  /// <param name="cancellationToken">Token to handle request cancellation.</param>
  /// <returns>The response containing SNMP data.</returns>
  async Task<SNMPResponse> IRequestHandler<SNMPGetCommand, SNMPResponse>.Handle(SNMPGetCommand request,
                                                                                CancellationToken cancellationToken)
    => await executor.GetCommand(request.Host,
                                 request.Community,
                                 request.OID,
                                 cancellationToken);
}

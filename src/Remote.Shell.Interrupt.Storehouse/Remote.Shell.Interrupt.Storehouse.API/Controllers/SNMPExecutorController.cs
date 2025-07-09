namespace Remote.Shell.Interrupt.Storehouse.API.Controllers;

/// <summary>
/// Provides endpoints for executing SNMP (Simple Network Management Protocol) commands.
/// </summary>
public class SNMPExecutorController : BaseAPIController
{
    /// <summary>
    /// Executes a single SNMP GET operation based on the provided command parameters.
    /// </summary>
    /// <param name="snmpGetCommand">The command containing target OID, host information, and credentials.</param>
    /// <param name="cancellationToken">Token used to cancel the request if needed.</param>
    /// <returns>
    /// A result containing the SNMP response payload. 
    /// Returns <c>200 OK</c> with a <see cref="SNMPResponse"/> on success, or <c>404 Not Found</c> if the device or OID is unreachable.
    /// </returns>
    [HttpGet]
    [ProducesResponseType(typeof(SNMPResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromBody] SNMPGetCommand snmpGetCommand,
                                         CancellationToken cancellationToken)
        => Ok(await Sender.Send(snmpGetCommand, cancellationToken));

    /// <summary>
    /// Executes an SNMP WALK operation to retrieve a sequence of OIDs from the target device.
    /// </summary>
    /// <param name="snmpWalkCommand">The command containing walk parameters such as root OID, target device, and credentials.</param>
    /// <param name="cancellationToken">Token used to cancel the request if needed.</param>
    /// <returns>
    /// A result containing a sequence of <see cref="SNMPResponse"/> objects. 
    /// Returns <c>200 OK</c> on success, or <c>404 Not Found</c> if the device or root OID cannot be resolved.
    /// </returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SNMPResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Walk([FromBody] SNMPWalkCommand snmpWalkCommand,
                                          CancellationToken cancellationToken)
    => Ok(await Sender.Send(snmpWalkCommand, cancellationToken));
}

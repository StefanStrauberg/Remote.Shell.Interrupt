namespace Remote.Shell.Interrupt.Storehouse.API.Controllers;

public class SNMPExecutorController : BaseAPIController
{
    [HttpGet]
    [ProducesResponseType(typeof(SNMPResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromBody] SNMPGetCommand snmpGetCommand,
                                         CancellationToken cancellationToken)
        => Ok(await Sender.Send(snmpGetCommand,
                                cancellationToken));

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SNMPResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Walk([FromBody] SNMPWalkCommand snmpWalkCommand,
                                          CancellationToken cancellationToken)
    => Ok( await Sender.Send(snmpWalkCommand,
                             cancellationToken));
}

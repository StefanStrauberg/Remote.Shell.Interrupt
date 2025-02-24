namespace Remote.Shell.Interrupt.Storehouse.API.Controllers;

public class SNMPExecutorController : BaseAPIController
{
    [HttpGet]
    [ProducesResponseType(typeof(SNMPResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromBody] SNMPGetCommand snmpGetCommand,
                                         CancellationToken cancellationToken)
    {
        var result = await Sender.Send(snmpGetCommand,
                                       cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SNMPResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Walk([FromBody] SNMPWalkCommand snmpWalkCommand,
                                          CancellationToken cancellationToken)
    {
        var result = await Sender.Send(snmpWalkCommand,
                                       cancellationToken);
        return Ok(result);
    }
}

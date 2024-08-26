namespace Remote.Shell.Interrupt.SNMPExecutor.API.Controllers;

public class SNMPExecutorController(ISender sender) : BaseAPIController
{
    readonly ISender _sender = sender
        ?? throw new ArgumentNullException(nameof(sender));

    [HttpGet]
    [ProducesResponseType(typeof(Info), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromBody] SNMPGetCommand snmpGetCommand,
                                         CancellationToken cancellationToken)
    {
        var result = await _sender.Send(snmpGetCommand,
                                        cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Info>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Walk([FromBody] SNMPWalkCommand snmpWalkCommand,
                                          CancellationToken cancellationToken)
    {
        var result = await _sender.Send(snmpWalkCommand,
                                        cancellationToken);
        return Ok(result);
    }
}

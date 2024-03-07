namespace Remote.Shell.Interrupt.SNMPExecutor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExecutorController(IMediator mediator) 
    : ControllerBase
{
    readonly IMediator _mediator = mediator
        ?? throw new ArgumentNullException(nameof(mediator));

    [HttpGet("Walk")]
    [ProducesResponseType(typeof(JsonObject), (int)HttpStatusCode.OK, "application/json")]
    public async Task<IActionResult> Walk(string host,
                                          string community,
                                          string oid)
        => Ok(await _mediator.Send(new SNMPWalkCommand(new SNMPParams(host,
                                                                      community),
                                                       oid)));
    
    [HttpGet("Get")]
    [ProducesResponseType(typeof(JsonObject), (int)HttpStatusCode.OK, "application/json")]
    public async Task<IActionResult> Get(string host,
                                         string community,
                                         string oid)
        => Ok(await _mediator.Send(new SNMPGetCommand(new SNMPParams(host,
                                                                     community),
                                                      oid)));
}

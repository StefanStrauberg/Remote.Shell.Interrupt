namespace Remote.Shell.Interrupt.SNMPExecutor.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ExecutorController(IMediator mediator) 
    : ControllerBase
{
    readonly IMediator _mediator = mediator
        ?? throw new ArgumentNullException(nameof(mediator));

    [HttpPost("SNMPWalk")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> SNMPWalk([FromBody] WalkCmdAndPrms prmsAndCmds)
        => Ok(await _mediator.Send(new SNMPWalkCommand(prmsAndCmds.ServerParams,
                                                       prmsAndCmds.Command)));
}

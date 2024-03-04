namespace Remote.Shell.Interrupt.SSHExecutor.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ExecutorController(IMediator mediator) 
    : ControllerBase
{
    readonly IMediator _mediator = mediator
        ?? throw new ArgumentNullException(nameof(mediator));

    [HttpPost("SSHExecute")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> SSHExecute([FromBody] ComposSrvPrmsAndCmds prmsAndCmds)
        => Ok(await _mediator.Send(new SSHExecuteCommands(prmsAndCmds.ServerParams,
                                                          prmsAndCmds.Commands)));
}

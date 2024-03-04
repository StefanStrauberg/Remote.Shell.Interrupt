namespace Remote.Shell.Interrupt.SSHExecutor.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class SShExecutorController(IMediator mediator) 
    : ControllerBase
{
    readonly IMediator _mediator = mediator
        ?? throw new ArgumentNullException(nameof(mediator));

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Execute([FromBody] CompositeSrvPrmsAndCmds prmsAndCmds)
        => Ok(await _mediator.Send(new ExecuteListCommands(prmsAndCmds.ServerParams,
                                                           prmsAndCmds.Commands)));
}

namespace Remote.Shell.Interrupt.SSHExecutor.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class SShExecutorController(IMediator mediator) 
    : ControllerBase
{
    readonly IMediator _mediator = mediator
        ?? throw new ArgumentNullException(nameof(mediator));

    [HttpPost("ExecuteCommand")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> ExecuteCommand([FromBody] CompositeSrvPrmsAndCmd prmsAndCmd)
        => Ok(await _mediator.Send(new ExecuteOneCommand(prmsAndCmd.ServerParams,
                                                         prmsAndCmd.Command)));

    [HttpPost("ExecuteCommands")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> ExecuteCommands([FromBody] CompositeSrvPrmsAndCmds prmsAndCmds)
        => Ok(await _mediator.Send(new ExecuteListCommands(prmsAndCmds.ServerParams,
                                                           prmsAndCmds.Command)));
}

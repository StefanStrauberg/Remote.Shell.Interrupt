namespace Remote.Shell.Interrupt.Storehouse.API.Controllers;

public class ClientCODsController : BaseAPIController
{
  [HttpGet]
  [ProducesResponseType(typeof(IEnumerable<ClientCODDTO>), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetClientsCOD(CancellationToken cancellationToken)
    => Ok(await Sender.Send(new GetClientsCODQuery(),
                            cancellationToken));

  [HttpGet("{name}")]
  [ProducesResponseType(typeof(IEnumerable<ClientCODDTO>), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetClientsCODByName(string name,
                                                       CancellationToken cancellationToken)
    => Ok(await Sender.Send(new GetClientsCODByNameQuery(name),
                            cancellationToken));

  [HttpGet("{vlanTag}")]
  [ProducesResponseType(typeof(IEnumerable<ClientCODDTO>), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetClientsCODByVlanTag(int vlanTag,
                                                          CancellationToken cancellationToken)
    => Ok(await Sender.Send(new GetClientsCODByVlanTagQuery(vlanTag),
                            cancellationToken));

  [HttpPut]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> UpdateOrganizations(CancellationToken cancellationToken)
    => Ok(await Sender.Send(new UpdateOrganizationsLocalDbCommand(),
                            cancellationToken));
}

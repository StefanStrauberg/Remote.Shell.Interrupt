namespace Remote.Shell.Interrupt.Storehouse.API.Controllers;

public class OrganizationsController(ISender sender) : BaseAPIController
{
  readonly ISender _sender = sender
    ?? throw new ArgumentNullException(nameof(sender));

  [HttpGet]
  [ProducesResponseType(typeof(IEnumerable<ClientCOD>), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetOrganizations(CancellationToken cancellationToken)
    => Ok(await _sender.Send(new GetOrganizationsQuery(),
                             cancellationToken));

  [HttpGet("{name}")]
  [ProducesResponseType(typeof(IEnumerable<ClientCOD>), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetOrganizationsByName(string name,
                                                          CancellationToken cancellationToken)
    => Ok(await _sender.Send(new GetOrganizationByNameQuery(name),
                             cancellationToken));

  [HttpGet("{vlanTag}")]
  [ProducesResponseType(typeof(IEnumerable<ClientCODDTO>), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetOrganizationsByVlanTag(int vlanTag,
                                                             CancellationToken cancellationToken)
    => Ok(await _sender.Send(new GetOrganizationByVlanTagQuery(vlanTag),
                             cancellationToken));

  [HttpPut]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> UpdateOrganizations(CancellationToken cancellationToken)
    => Ok(await _sender.Send(new UpdateOrganizationsLocalDbCommand(), cancellationToken));
}

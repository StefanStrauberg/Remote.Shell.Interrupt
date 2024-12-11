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
}

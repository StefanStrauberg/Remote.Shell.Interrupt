using System.Text.Json;

namespace Remote.Shell.Interrupt.Storehouse.API.Controllers;

public class ClientCODsController : BaseAPIController
{
  [HttpGet]
  [ProducesResponseType(typeof(IEnumerable<ClientCODDTO>), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetClientsCOD([FromQuery] RequestParameters requestParameters,CancellationToken cancellationToken)
  {
    var result = await Sender.Send(new GetClientsCODQuery(requestParameters),
                                   cancellationToken);
    var metadata = new
    {
        result.TotalCount,
        result.PageSize,
        result.CurrentPage,
        result.TotalPages,
        result.HasNext,
        result.HasPrevious
    };
    Response.Headers
            .Append("X-Pagination",
                    JsonSerializer.Serialize(metadata));
    return Ok(result);
  }

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

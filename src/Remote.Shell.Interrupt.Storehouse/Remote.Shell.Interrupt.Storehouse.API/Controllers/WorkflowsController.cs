using Microsoft.AspNetCore.Authorization;

namespace Remote.Shell.Interrupt.Storehouse.API.Controllers;

/// <summary>
/// Provides API endpoints for managing workflow graphs (nodes/edges) and running them
/// against a live device over SNMP.
/// </summary>
[Authorize(Roles = "Admin")]
public class WorkflowsController(ISender sender) : BaseAPIController(sender)
{
  /// <summary>
  /// Retrieves a filtered and paginated list of workflow summaries.
  /// </summary>
  /// <param name="requestParameters">Contains filtering and pagination options.</param>
  /// <param name="cancellationToken">Token to cancel the request if needed.</param>
  /// <returns>
  /// A paginated collection of <see cref="WorkflowSummaryDTO"/>s with metadata in the <c>X-Pagination</c> response header.
  /// </returns>
  [HttpGet]
  [ProducesResponseType(typeof(IEnumerable<WorkflowSummaryDTO>), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetWorkflowsByFilter([FromQuery] RequestParameters requestParameters,
                                                        CancellationToken cancellationToken)
  {
    var result = await Sender.Send(new GetWorkflowsByFilterQuery(requestParameters), cancellationToken);
    var metadata = new PaginationMetadata()
    {
      TotalCount = result.TotalCount,
      PageSize = result.PageSize,
      CurrentPage = result.CurrentPage,
      TotalPages = result.TotalPages,
      HasNext = result.HasNext,
      HasPrevious = result.HasPrevious
    };
    Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));
    return Ok(result);
  }

  /// <summary>
  /// Retrieves a single workflow, including its full node/edge graph, by its unique identifier.
  /// </summary>
  /// <param name="id">The unique ID of the workflow to retrieve.</param>
  /// <param name="cancellationToken">Token to cancel the request if needed.</param>
  /// <returns>
  /// A <see cref="WorkflowDTO"/> object if found, or <see cref="ApiErrorResponse"/> with status <c>404</c> if not.
  /// </returns>
  [HttpGet("{id:guid}")]
  [ProducesResponseType(typeof(WorkflowDTO), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> GetWorkflowById(Guid id, CancellationToken cancellationToken)
    => Ok(await Sender.Send(new GetWorkflowByIdQuery(id), cancellationToken));

  /// <summary>
  /// Creates a new workflow graph.
  /// </summary>
  /// <param name="createWorkflowDTO">The graph (name, start node, nodes, edges) to create.</param>
  /// <param name="cancellationToken">Token to cancel the request if needed.</param>
  /// <returns>The new workflow's ID.</returns>
  [HttpPost]
  [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
  public async Task<IActionResult> CreateWorkflow([FromBody] CreateWorkflowDTO createWorkflowDTO,
                                                  CancellationToken cancellationToken)
    => Ok(await Sender.Send(new CreateWorkflowCommand(createWorkflowDTO), cancellationToken));

  /// <summary>
  /// Replaces an existing workflow's graph (name, start node, nodes, edges).
  /// </summary>
  /// <param name="updateWorkflowDTO">The full replacement graph, including the workflow's Id.</param>
  /// <param name="cancellationToken">Token to cancel the request if needed.</param>
  /// <returns>
  /// <see cref="StatusCodes.Status200OK"/> on success, or <see cref="ApiErrorResponse"/> with status <c>404</c> if not found.
  /// </returns>
  [HttpPut]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> UpdateWorkflow([FromBody] UpdateWorkflowDTO updateWorkflowDTO,
                                                  CancellationToken cancellationToken)
    => Ok(await Sender.Send(new UpdateWorkflowCommand(updateWorkflowDTO), cancellationToken));

  /// <summary>
  /// Deletes a workflow by its unique identifier. Its nodes and edges are removed with it.
  /// </summary>
  /// <param name="id">The ID of the workflow to delete.</param>
  /// <param name="cancellationToken">Token to cancel the request if needed.</param>
  /// <returns>
  /// <see cref="StatusCodes.Status200OK"/> on successful deletion, or <see cref="ApiErrorResponse"/> with status <c>404</c> if not found.
  /// </returns>
  [HttpDelete("{id:guid}")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> DeleteWorkflowById(Guid id, CancellationToken cancellationToken)
    => Ok(await Sender.Send(new DeleteWorkflowCommand(id), cancellationToken));

  /// <summary>
  /// Runs a workflow against a live device over SNMP.
  /// </summary>
  /// <param name="id">The ID of the workflow to run.</param>
  /// <param name="request">The target device's Host/Community.</param>
  /// <param name="cancellationToken">Token to cancel the request if needed.</param>
  /// <returns>
  /// A <see cref="WorkflowExecutionResultDTO"/> describing the run, including a step-by-step
  /// trace. A workflow-level failure (bad OID, unreachable device, an unresolved decision) is
  /// reported as <c>Success:false</c> within this <c>200 OK</c> body, not as an HTTP error -
  /// the partial trace up to the failing node is the point of surfacing it.
  /// </returns>
  [HttpPost("{id:guid}")]
  [ProducesResponseType(typeof(WorkflowExecutionResultDTO), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> ExecuteWorkflow(Guid id,
                                                    [FromBody] ExecuteWorkflowRequestDTO request,
                                                    CancellationToken cancellationToken)
    => Ok(await Sender.Send(new ExecuteWorkflowCommand(id, request.Host, request.Community, request.Input), cancellationToken));

  /// <summary>
  /// Publishes a Draft workflow, freezing its graph. Once Published, <see cref="UpdateWorkflow"/> refuses it.
  /// </summary>
  /// <param name="id">The ID of the workflow to publish.</param>
  /// <param name="cancellationToken">Token to cancel the request if needed.</param>
  /// <returns>
  /// <see cref="StatusCodes.Status200OK"/> on success, <see cref="ApiErrorResponse"/> with status <c>404</c> if not
  /// found, or <c>400</c> if the workflow isn't currently Draft.
  /// </returns>
  [HttpPost("{id:guid}")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> PublishWorkflow(Guid id, CancellationToken cancellationToken)
    => Ok(await Sender.Send(new PublishWorkflowCommand(id), cancellationToken));

  /// <summary>
  /// Archives a workflow (Draft or Published).
  /// </summary>
  /// <param name="id">The ID of the workflow to archive.</param>
  /// <param name="cancellationToken">Token to cancel the request if needed.</param>
  /// <returns>
  /// <see cref="StatusCodes.Status200OK"/> on success, <see cref="ApiErrorResponse"/> with status <c>404</c> if not
  /// found, or <c>400</c> if it's already Archived.
  /// </returns>
  [HttpPost("{id:guid}")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> ArchiveWorkflow(Guid id, CancellationToken cancellationToken)
    => Ok(await Sender.Send(new ArchiveWorkflowCommand(id), cancellationToken));
}

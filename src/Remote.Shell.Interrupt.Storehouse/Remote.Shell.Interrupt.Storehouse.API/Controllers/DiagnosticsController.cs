using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

namespace Remote.Shell.Interrupt.Storehouse.API.Controllers;

/// <summary>
/// Receives diagnostics reported by the SPA itself, outside of any user-initiated request.
/// </summary>
public class DiagnosticsController(ISender sender) : BaseAPIController(sender)
{
  /// <summary>
  /// Records an unhandled error caught by the SPA's error boundaries into the backend's own
  /// structured logs - see ReportClientErrorCommand for why that, rather than the browser
  /// console, is this app's actual monitoring destination.
  /// </summary>
  /// <param name="command">The error and whatever context the reporting boundary captured.</param>
  /// <param name="cancellationToken">Token to cancel the request if needed.</param>
  /// <returns><see cref="StatusCodes.Status200OK"/> once the error has been logged.</returns>
  [HttpPost]
  [AllowAnonymous]
  [EnableRateLimiting(DefaultEntities.ClientErrorRateLimitPolicy)]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public async Task<IActionResult> ReportClientError([FromBody] ReportClientErrorCommand command,
                                                      CancellationToken cancellationToken)
  {
    await Sender.Send(command, cancellationToken);
    return Ok();
  }
}

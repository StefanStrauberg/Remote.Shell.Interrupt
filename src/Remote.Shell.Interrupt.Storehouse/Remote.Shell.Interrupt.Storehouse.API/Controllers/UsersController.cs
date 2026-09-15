using Microsoft.AspNetCore.Authorization;
using Remote.Shell.Interrupt.Storehouse.Application.DTOs.Users;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Users.Commands.DeleteUser;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Users.Commands.SetUserActive;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Users.Commands.UpdateUserRole;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Users.Queries.GetUsersByFilter;

namespace Remote.Shell.Interrupt.Storehouse.API.Controllers;

/// <summary>
/// Provides API endpoints for administering application accounts: listing,
/// role changes, activation/deactivation, and deletion. Administrators only -
/// new accounts are created via <see cref="AuthController.Register"/>.
/// </summary>
[Authorize(Roles = "Admin")]
public class UsersController(ISender sender) : BaseAPIController(sender)
{
  /// <summary>
  /// Retrieves a filtered and paginated list of application accounts.
  /// </summary>
  [HttpGet]
  [ProducesResponseType(typeof(IEnumerable<UserDTO>), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetUsersByFilter([FromQuery] RequestParameters requestParameters,
                                                     CancellationToken cancellationToken)
  {
    var result = await Sender.Send(new GetUsersByFilterQuery(requestParameters), cancellationToken);
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
  /// Replaces every role held by an account with a single new one ("Admin" or "User").
  /// </summary>
  [HttpPut]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> UpdateUserRole([FromBody] UpdateUserRoleCommand updateUserRoleCommand,
                                                  CancellationToken cancellationToken)
    => Ok(await Sender.Send(updateUserRoleCommand, cancellationToken));

  /// <summary>
  /// Activates or deactivates an account. A deactivated account fails login
  /// even with valid credentials, but its data and history are kept.
  /// </summary>
  [HttpPut]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> SetUserActive([FromBody] SetUserActiveCommand setUserActiveCommand,
                                                 CancellationToken cancellationToken)
    => Ok(await Sender.Send(setUserActiveCommand, cancellationToken));

  /// <summary>
  /// Permanently deletes an account.
  /// </summary>
  [HttpDelete("{id:guid}")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
  public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
    => Ok(await Sender.Send(new DeleteUserCommand(id), cancellationToken));
}

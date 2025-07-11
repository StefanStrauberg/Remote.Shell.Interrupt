namespace Remote.Shell.Interrupt.Storehouse.API.Controllers;

/// <summary>
/// Provides a base class for API controllers that utilize mediator-based request handling.
/// Configured with routing conventions for <c>[controller]/[action]</c>.
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
public class BaseAPIController(ISender sender) : ControllerBase
{
  /// <summary>
  /// Lazily resolves an <see cref="ISender"/> instance from the current request's service provider.
  /// Used to dispatch commands and queries via MediatR.
  /// </summary>
  /// <exception cref="ArgumentNullException">
  /// Thrown if the <see cref="ISender"/> service is not available in the request context.
  /// </exception>
  protected ISender Sender = sender
    ?? throw new ArgumentNullException(nameof(sender));
}

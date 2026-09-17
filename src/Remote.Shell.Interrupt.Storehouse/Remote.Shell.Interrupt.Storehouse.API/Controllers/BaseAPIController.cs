namespace Remote.Shell.Interrupt.Storehouse.API.Controllers;

/// <summary>
/// Provides a base class for API controllers that utilize mediator-based request handling.
/// Configured with routing conventions for <c>v1/[controller]/[action]</c>.
/// </summary>
/// <remarks>
/// The <c>v1</c> segment is a plain route literal rather than a full API-versioning package
/// (e.g. Asp.Versioning.Mvc): with a single version in production there is nothing yet for
/// version negotiation, deprecation headers, or per-version Swagger docs to do. What matters
/// today is only that the URL space has room for a "v2" controller to sit next to this one
/// once a breaking contract change actually happens, without moving every existing route out
/// from under current clients.
/// </remarks>
[ApiController]
[Route("api/v1/[controller]/[action]")]
public class BaseAPIController(ISender sender) : ControllerBase
{
  /// <summary>
  /// The injected <see cref="ISender"/> derived controllers use to dispatch commands and
  /// queries via Mediator.
  /// </summary>
  /// <exception cref="ArgumentNullException">
  /// Thrown at construction if the injected <see cref="ISender"/> is <c>null</c>.
  /// </exception>
  protected ISender Sender = sender
    ?? throw new ArgumentNullException(nameof(sender));
}

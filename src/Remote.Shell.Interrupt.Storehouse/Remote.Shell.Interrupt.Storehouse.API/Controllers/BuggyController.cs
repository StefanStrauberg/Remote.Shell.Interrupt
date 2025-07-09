namespace Remote.Shell.Interrupt.Storehouse.API.Controllers;

/// <summary>
/// Controller for simulating common HTTP error responses for testing and diagnostics.
/// </summary>
public class BuggyController : BaseAPIController
{
    /// <summary>
    /// Returns a simulated 404 Not Found response.
    /// </summary>
    /// <returns><see cref="NotFoundResult"/> with a test payload.</returns>
    [HttpGet]
    public IActionResult GetNotFound()
        => NotFound(new ApiErrorResponse(404, "Not Found", "Testing a not found error", null));

    /// <summary>
    /// Returns a simulated 400 Bad Request response.
    /// </summary>
    /// <returns><see cref="BadRequestObjectResult"/> with a test payload.</returns>
    [HttpGet]
    public IActionResult GetBadRequest()
        => BadRequest(new ApiErrorResponse(400, "Bad Request", "Testing a bad request", null));

    /// <summary>
    /// Throws an unhandled exception to simulate a 500 Internal Server Error.
    /// </summary>
    /// <returns>This method does not return; it throws.</returns>
    [HttpGet]
    public IActionResult GetServerError()
    {
        throw new Exception("Testing a server error");
    }

    /// <summary>
    /// Returns a simulated 401 Unauthorized response.
    /// </summary>
    /// <returns><see cref="UnauthorizedResult"/> with a test payload.</returns>
    [HttpGet]
    public IActionResult GetUnauthorized()
        => Unauthorized(new ApiErrorResponse(401, "Unauthorized", "Testing  an unauthorized error", null));
}

namespace Remote.Shell.Interrupt.Storehouse.API.Controllers;

public class BuggyController : BaseAPIController
{
    [HttpGet]
    public IActionResult GetNotFound()
        => NotFound(new ApiErrorResponse(404, "Not Found", "Testing a not found error", null));

    [HttpGet]
    public IActionResult GetBadRequest()
        => BadRequest(new ApiErrorResponse(400, "Bad Request", "Testing a bad request", null));

    [HttpGet]
    public IActionResult GetServerError()
    {
        throw new Exception("Testing a server error");
    }

    [HttpGet]
    public IActionResult GetUnauthorized()
        => Unauthorized(new ApiErrorResponse(401, "Unauthorized", "Testing  an unauthorized error", null));
}

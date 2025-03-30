namespace Remote.Shell.Interrupt.Storehouse.API.Controllers;

public class BuggyController : BaseAPIController
{
    [HttpGet("not-found")]
    public ActionResult GetNotFound()
        => NotFound("Testing a not found error");

    [HttpGet("bad-request")]
    public ActionResult GetBadRequest()
        => BadRequest("Testing a bad request");

    [HttpGet("server-error")]
    public ActionResult GetServerError()
    {
        throw new Exception("Testing a server error");
    }

    [HttpGet("unauthorised")]
    public ActionResult GetUnauthorised()
        => Unauthorized("Testing an unauthorized error");
}

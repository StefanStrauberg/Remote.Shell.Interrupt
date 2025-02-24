namespace Remote.Shell.Interrupt.Storehouse.API.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class BaseAPIController : ControllerBase
{
  protected ISender Sender => HttpContext.RequestServices.GetService<ISender>()
    ?? throw new ArgumentNullException("ISender service is unavailable");
}

namespace Remote.Shell.Interrupt.Storehouse.Application.Middleware;

public class ExceptionHandlingMiddleware(IAppLogger<ExceptionHandlingMiddleware> logger) : IMiddleware
{
  readonly IAppLogger<ExceptionHandlingMiddleware> _logger = logger
    ?? throw new ArgumentNullException(nameof(logger));

  async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
  {
    try
    {
      await next(context);
    }
    catch (Exception e)
    {
      _logger.LogError(e.ToString());
      await HandleExceptionAsync(context, e);
    }
  }

  static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
  {
    var statusCode = GetStatusCode(exception);
    var response = new ApiErrorResponse(status: statusCode,
                                        title: GetTitle(exception),
                                        detail: exception.Message,
                                        errors: GetErrors(exception));
    httpContext.Response.ContentType = "application/json";
    httpContext.Response.StatusCode = statusCode;
    await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
  }

  static int GetStatusCode(Exception exception) =>
      exception switch
      {
        BadRequestException => StatusCodes.Status400BadRequest,
        NotFoundException => StatusCodes.Status404NotFound,
        ValidationException => StatusCodes.Status422UnprocessableEntity,
        _ => StatusCodes.Status500InternalServerError
      };

  static string GetTitle(Exception exception) =>
      exception switch
      {
        ApplicationException applicationException => applicationException.Title,
        _ => "Server Error"
      };

  static IReadOnlyDictionary<string, string[]>? GetErrors(Exception exception)
  {
    IReadOnlyDictionary<string, string[]>? errors = null;

    if (exception is ValidationException validationException)
    {
      errors = validationException.ErrorsDictionary;
    }

    return errors;
  }
}

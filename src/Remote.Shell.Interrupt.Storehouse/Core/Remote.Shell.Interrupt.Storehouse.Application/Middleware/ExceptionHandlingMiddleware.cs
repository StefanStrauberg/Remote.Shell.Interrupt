namespace Remote.Shell.Interrupt.Storehouse.Application.Middleware;

/// <summary>
/// Middleware for handling exceptions and returning structured error responses.
/// </summary>
/// <param name="logger">Logger for recording error details.</param>
public class ExceptionHandlingMiddleware(IAppLogger<ExceptionHandlingMiddleware> logger) : IMiddleware
{
  /// <summary>
  /// The logger instance used for error logging.
  /// </summary>
  readonly IAppLogger<ExceptionHandlingMiddleware> _logger = logger
    ?? throw new ArgumentNullException(nameof(logger));

  /// <summary>
  /// Invokes the next middleware component and handles exceptions globally.
  /// </summary>
  /// <param name="context">The HTTP context of the request.</param>
  /// <param name="next">The delegate for the next middleware in the pipeline.</param>
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

  /// <summary>
  /// Handles an exception and returns a structured API error response.
  /// </summary>
  /// <param name="httpContext">The HTTP context.</param>
  /// <param name="exception">The exception to handle.</param>
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

  /// <summary>
  /// Determines the appropriate HTTP status code based on the exception type.
  /// </summary>
  /// <param name="exception">The exception to evaluate.</param>
  /// <returns>The corresponding HTTP status code.</returns>
  static int GetStatusCode(Exception exception) =>
      exception switch
      {
        BadRequestException => StatusCodes.Status400BadRequest,
        NotFoundException => StatusCodes.Status404NotFound,
        ValidationException => StatusCodes.Status422UnprocessableEntity,
        _ => StatusCodes.Status500InternalServerError
      };

  /// <summary>
  /// Retrieves the error title based on the exception type.
  /// </summary>
  /// <param name="exception">The exception to evaluate.</param>
  /// <returns>The error title string.</returns>
  static string GetTitle(Exception exception) =>
      exception switch
      {
        ApplicationException applicationException => applicationException.Title,
        _ => "Server Error"
      };

  /// <summary>
  /// Extracts validation errors from the exception if applicable.
  /// </summary>
  /// <param name="exception">The exception to check for validation errors.</param>
  /// <returns>A dictionary of validation errors, if available; otherwise, null.</returns>
  static IReadOnlyDictionary<string, string[]>? GetErrors(Exception exception)
  {
    IReadOnlyDictionary<string, string[]>? errors = null;

    if (exception is ValidationException validationException)
      errors = validationException.ErrorsDictionary;

    return errors;
  }
}

namespace Remote.Shell.Interrupt.Storehouse.Application.Exceptions;

/// <summary>
/// Represents an error response for the API.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ApiErrorResponse"/> class.
/// </remarks>
public class ApiErrorResponse(int status,
                              string title,
                              string detail,
                              IReadOnlyDictionary<string, string[]>? errors = null)
{
    /// <summary>
    /// Gets or sets the title of the error.
    /// </summary>
    public string Title { get; set; } = title;

    /// <summary>
    /// Gets or sets the HTTP status code associated with the error.
    /// </summary>
    public int Status { get; set; } = status;

    /// <summary>
    /// Gets or sets detailed information about the error.
    /// </summary>
    public string Detail { get; set; } = detail;

    /// <summary>
    /// Gets or sets a dictionary of validation errors, if any.
    /// </summary>
    public IReadOnlyDictionary<string, string[]>? Errors { get; set; } = errors;

    /// <summary>
    /// Creates a generic error response.
    /// </summary>
    public static ApiErrorResponse CreateGenericError(int status, string message)
        => new ApiErrorResponse(status, "An error occurred", message);
}

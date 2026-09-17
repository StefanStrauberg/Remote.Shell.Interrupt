using Mediator;

namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Diagnostics.Commands.ReportClientError;

/// <summary>
/// An unhandled error caught by the SPA's error boundaries (see
/// ApplicationErrorBoundary.tsx and RouteErrorPage.tsx on the client), reported so it lands
/// in this app's actual production monitoring - the backend's structured logs (see
/// CorrelationIdMiddleware) - instead of only the browser console, which nobody watches once
/// the app is running on a real user's machine. <see cref="Context"/> carries whatever
/// boundary-specific details the caller has (component stack, route status, ...) as a raw
/// JSON object, rather than a fixed set of fields, so new callers don't need a contract change.
/// </summary>
public record ReportClientErrorCommand(string Message,
                                        string? Stack,
                                        string? Url,
                                        string? UserAgent,
                                        string? Context) : CQRS.ICommand<Unit>;

internal class ReportClientErrorCommandHandler(IAppLogger<ReportClientErrorCommandHandler> logger)
  : CQRS.ICommandHandler<ReportClientErrorCommand, Unit>
{
  public ValueTask<Unit> Handle(ReportClientErrorCommand request, CancellationToken cancellationToken)
  {
    logger.LogError(
      "[Client] {Message} at {Url} ({UserAgent}) context={Context}{NewLine}{Stack}",
      request.Message,
      request.Url ?? "unknown",
      request.UserAgent ?? "unknown",
      request.Context ?? "{}",
      Environment.NewLine,
      request.Stack ?? "no stack trace");

    return Unit.ValueTask;
  }
}

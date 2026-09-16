using Remote.Shell.Interrupt.Storehouse.Application.Features.Diagnostics.Commands.ReportClientError;

namespace Remote.Shell.Interrupt.Storehouse.Application.Validations.Diagnostics;

/// <summary>
/// Bounds every field of an anonymously-submitted client error report: the endpoint writes
/// straight into the backend's log file (see DefaultEntities.LoggingTo), so an unbounded
/// payload from an unauthenticated caller is a disk-filling denial-of-service vector, not just
/// a formatting concern.
/// </summary>
public class ReportClientErrorCommandValidator : AbstractValidator<ReportClientErrorCommand>
{
  public ReportClientErrorCommandValidator()
  {
    RuleFor(x => x.Message).NotEmpty().WithMessage("Message can't be empty")
                            .MaximumLength(2_000);
    RuleFor(x => x.Stack).MaximumLength(8_000);
    RuleFor(x => x.Url).MaximumLength(2_000);
    RuleFor(x => x.UserAgent).MaximumLength(500);
    RuleFor(x => x.Context).MaximumLength(4_000);
  }
}

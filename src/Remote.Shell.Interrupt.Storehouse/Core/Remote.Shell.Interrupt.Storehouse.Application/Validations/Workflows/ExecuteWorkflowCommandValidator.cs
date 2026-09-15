using Remote.Shell.Interrupt.Storehouse.Application.Validations.SNMP;

namespace Remote.Shell.Interrupt.Storehouse.Application.Validations.Workflows;

/// <summary>
/// Validates the SNMP target on a workflow run request. Mirrors
/// <see cref="SNMPGetCommandValidator"/>/<see cref="SNMPWalkCommandValidator"/> exactly - a
/// workflow's <c>SnmpGet</c>/<c>SnmpWalk</c> nodes ultimately hit the same
/// <see cref="Remote.Shell.Interrupt.Storehouse.Infrastructure.SNMPCommandExecutor.SNMPCommandExecutor"/>
/// against <see cref="ExecuteWorkflowCommand.Host"/>, so it needs the same format and
/// SSRF-shaped-target checks, not just the "AbstractValidator exists for every SNMP entry
/// point" guarantee those two already gave the Get/Walk endpoints.
/// </summary>
public class ExecuteWorkflowCommandValidator : AbstractValidator<ExecuteWorkflowCommand>
{
  public ExecuteWorkflowCommandValidator()
  {
    RuleFor(x => x.Host).NotNull().WithMessage("Host can't be null")
                        .NotEmpty().WithMessage("Host can't be empty")
                        .Matches(@"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$")
                        .WithMessage("Invalid host. Host should be IPv4 address")
                        .Must(SnmpTargetGuard.IsAllowedTarget)
                        .WithMessage("Invalid host. Loopback, link-local, multicast and reserved addresses are not valid SNMP targets");

    RuleFor(x => x.Community).NotNull().WithMessage("Community can't be null")
                             .NotEmpty().WithMessage("Community can't be empty");
  }
}

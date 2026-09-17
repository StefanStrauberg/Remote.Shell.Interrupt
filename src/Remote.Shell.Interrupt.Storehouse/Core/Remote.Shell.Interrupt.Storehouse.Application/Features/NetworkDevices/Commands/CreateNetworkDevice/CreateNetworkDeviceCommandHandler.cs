using Mediator;

namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Commands.CreateNetworkDevice;

public record CreateNetworkDeviceCommand(string Host, string Community, string TypeOfNetworkDevice) : CQRS.ICommand;

/// <summary>
/// Runs the seeded "Network device discovery" workflow (see
/// <see cref="DefaultNetworkDeviceWorkflowFactory"/>) against the given device and saves the
/// result. This used to be ~900 lines of hand-coded, vendor-branching SNMP orchestration
/// directly in this handler - that logic now lives entirely in the workflow graph (Script
/// nodes for the per-vendor transforms, a SaveNetworkDevice node for persistence), seeded
/// once at startup by <c>WorkflowSeeder</c>. This handler is now just the thin adapter
/// between the existing public API contract and the engine.
/// </summary>
internal class CreateNetworkDeviceCommandHandler(IWorkflowUnitOfWork workflowUnitOfWork,
                                                 IWorkflowSpecification specification,
                                                 IWorkflowEngine engine)
  : CQRS.ICommandHandler<CreateNetworkDeviceCommand, Unit>
{
  async ValueTask<Unit> IRequestHandler<CreateNetworkDeviceCommand, Unit>.Handle(CreateNetworkDeviceCommand request,
                                                                            CancellationToken cancellationToken)
  {
    // Fail fast, before any SNMP call, exactly like the handler this replaces did.
    if (!Enum.TryParse<TypeOfNetworkDevice>(request.TypeOfNetworkDevice, ignoreCase: true, out _))
      throw new BadRequestException(
        $"Unknown network device type '{request.TypeOfNetworkDevice}'. Supported types: {string.Join(", ", Enum.GetNames<TypeOfNetworkDevice>())}.");

    var spec = specification.Clone();
    spec.AddFilter(w => w.Id == DefaultNetworkDeviceWorkflowFactory.WorkflowId);
    spec.AddInclude(w => w.Nodes);
    spec.AddInclude(w => w.Edges);

    var workflow = await workflowUnitOfWork.Workflows.GetOneWithChildrenAsync(spec, cancellationToken);

    var context = new WorkflowContext(request.Host, request.Community, workflow);
    context.Set("input.vendor", request.TypeOfNetworkDevice);

    var result = await engine.ExecuteAsync(workflow, context, cancellationToken);

    if (!result.Success)
      throw new InvalidOperationException(result.Error);

    return Unit.Value;
  }
}

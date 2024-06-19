namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Gateways;

public record CreateManyGatewaysCommand(IEnumerable<IPAddress> Hosts) : ICommand;

public class CreateManyGatewaysCommandHandler(IGatewayRepository gatewayRepository)
  : ICommandHandler<CreateManyGatewaysCommand, Unit>
{
  readonly IGatewayRepository _gatewayRepository = gatewayRepository
    ?? throw new ArgumentNullException(nameof(gatewayRepository));

  async Task<Unit> IRequestHandler<CreateManyGatewaysCommand, Unit>.Handle(CreateManyGatewaysCommand request,
                                                                           CancellationToken cancellationToken)
  {
    var gateways = request.Adapt<IEnumerable<Gateway>>();

    await _gatewayRepository.InsertManyAsync(gateways,
                                             cancellationToken);
    return Unit.Value;
  }
}

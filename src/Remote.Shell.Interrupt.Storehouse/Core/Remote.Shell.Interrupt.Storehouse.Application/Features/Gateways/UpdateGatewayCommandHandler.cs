namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Gateways;

public record UpdateGatewayCommand(Guid Id, Gateway Gateway) : ICommand;

internal class UpdateGatewayCommandHandler(IGatewayRepository gatewayRepository)
  : ICommandHandler<UpdateGatewayCommand, Unit>
{
  readonly IGatewayRepository _gatewayRepository = gatewayRepository
    ?? throw new ArgumentNullException(nameof(gatewayRepository));

  async Task<Unit> IRequestHandler<UpdateGatewayCommand, Unit>.Handle(UpdateGatewayCommand request,
                                                                      CancellationToken cancellationToken)
  {
    var existUpdatingGateway = await _gatewayRepository.ExistsAsync(x => x.Id == request.Id,
                                                                    cancellationToken);

    if (!existUpdatingGateway)
      throw new GatewayNotFoundException(request.Id.ToString());

    var updateGateway = request.Adapt<Gateway>();

    await _gatewayRepository.ReplaceOneAsync(x => x.Id == request.Id,
                                             updateGateway,
                                             cancellationToken);
    return Unit.Value;
  }
}

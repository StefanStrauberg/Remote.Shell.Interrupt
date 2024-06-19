namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Gateways;

public record CreateGatewayCommand(IPAddress Host) : ICommand;

internal class CreateGatewayCommandHandler(IGatewayRepository gatewayRepository)
  : ICommandHandler<CreateGatewayCommand, Unit>
{
  readonly IGatewayRepository _gatewayRepository = gatewayRepository
    ?? throw new ArgumentNullException(nameof(gatewayRepository));

  async Task<Unit> IRequestHandler<CreateGatewayCommand, Unit>.Handle(CreateGatewayCommand request,
                                                                      CancellationToken cancellationToken)
  {
    var gateway = request.Adapt<Gateway>();

    await _gatewayRepository.InsertOneAsync(gateway,
                                            cancellationToken);
    return Unit.Value;
  }
}

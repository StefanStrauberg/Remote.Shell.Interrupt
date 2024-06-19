namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Gateways;

public record DeleteGatewayCommand(Guid Id) : ICommand;

internal class DeleteGatewayCommandHandler(IGatewayRepository gatewayRepository)
  : ICommandHandler<DeleteGatewayCommand, Unit>
{
  readonly IGatewayRepository _gatewayRepository = gatewayRepository
    ?? throw new ArgumentNullException(nameof(gatewayRepository));

  async Task<Unit> IRequestHandler<DeleteGatewayCommand, Unit>.Handle(DeleteGatewayCommand request,
                                                                      CancellationToken cancellationToken)
  {
    var existDeletingGateway = await _gatewayRepository.ExistsAsync(x => x.Id == request.Id,
                                                                    cancellationToken);

    if (!existDeletingGateway)
      throw new GatewayNotFoundException(request.Id.ToString());

    await _gatewayRepository.DeleteOneAsync(x => x.Id == request.Id,
                                            cancellationToken);
    return Unit.Value;
  }
}

namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Gateways;

public record GetGatewaysQuery : IQuery<IEnumerable<Gateway>>;

internal class GetGatewaysQueryHandler(IGatewayRepository gatewayRepository)
  : IQueryHandler<GetGatewaysQuery, IEnumerable<Gateway>>
{
  readonly IGatewayRepository _gatewayRepository = gatewayRepository
    ?? throw new ArgumentNullException(nameof(gatewayRepository));

  async Task<IEnumerable<Gateway>> IRequestHandler<GetGatewaysQuery, IEnumerable<Gateway>>.Handle(GetGatewaysQuery request,
                                                                                                  CancellationToken cancellationToken)
  {
    var gateways = await _gatewayRepository.GetAllAsync(cancellationToken);

    return gateways;
  }
}

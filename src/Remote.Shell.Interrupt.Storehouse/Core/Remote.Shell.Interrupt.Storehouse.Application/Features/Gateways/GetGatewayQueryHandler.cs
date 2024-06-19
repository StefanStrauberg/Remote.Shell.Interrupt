namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Gateways;

public record GetGatewayQuery(Guid Id) : IQuery<Gateway>;

internal class GetGatewayQueryHandler(IGatewayRepository gatewayRepository)
  : IRequestHandler<GetGatewayQuery, Gateway>
{
  readonly IGatewayRepository _gatewayRepository = gatewayRepository
    ?? throw new ArgumentNullException(nameof(gatewayRepository));

  async Task<Gateway> IRequestHandler<GetGatewayQuery, Gateway>.Handle(GetGatewayQuery request,
                                                                       CancellationToken cancellationToken)
  {
    var gateway = await _gatewayRepository.FindOneAsync(x => x.Id == request.Id, cancellationToken)
      ?? throw new GatewayNotFoundException(request.Id.ToString());

    return gateway;
  }
}

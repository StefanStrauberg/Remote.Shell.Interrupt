namespace Remote.Shell.Interrupt.Storehouse.Application.Features.NetworkDevices.Queries.GetPorts;

// Запрос на получение портов по идентификаторам... О, черт, это звучит скучно! Но ладно, давайте сделаем это.
public record GetPortsByIdsQuery(IEnumerable<Guid> Ids) : IQuery<IEnumerable<PortDTO>>;

internal class GetPortsByIdsQueryHandler(INetDevUnitOfWork netDevUnitOfWork,
                                         IMapper mapper)
  : IQueryHandler<GetPortsByIdsQuery, IEnumerable<PortDTO>>
{
  async Task<IEnumerable<PortDTO>> IRequestHandler<GetPortsByIdsQuery, IEnumerable<PortDTO>>.Handle(GetPortsByIdsQuery request,
                                                                                                    CancellationToken cancellationToken)
  {
    // Чего, список идентификаторов пустой? Зачем ты вообще что-то запрашиваешь?!
    if(!request.Ids.Any())
      return [];

    // Ладно, загружаем порты. Надеюсь, эти порты не вызовут космическую аномалию!
    var ports = await netDevUnitOfWork.Ports
                                      .GetAllAggregatedPortsByListAsync(request.Ids,
                                                                        cancellationToken);

    // Преобразуем их в DTO. Если это не работает, ну, просто притворись, что всё нормально!
    return mapper.Map<IEnumerable<PortDTO>>(ports);
  }
}


namespace Remote.Shell.Interrupt.Storehouse.Application.Features.SPRVlans.Queries.GetAll;

// Запрос для получения списка SPR VLAN.
public record GetSPRVlansQuery(RequestParameters RequestParameters) 
  : IQuery<PagedList<SPRVlanDTO>>;

internal class GetSPRVlansQueryHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                       IMapper mapper) 
  : IQueryHandler<GetSPRVlansQuery, PagedList<SPRVlanDTO>>
{
  async Task<PagedList<SPRVlanDTO>> IRequestHandler<GetSPRVlansQuery, PagedList<SPRVlanDTO>>.Handle(GetSPRVlansQuery request,
                                                                                                    CancellationToken cancellationToken)
  {
    // Загружаем список SPR VLAN из базы данных.
    var sprVlans = await locBillUnitOfWork.SPRVlans
                                          .GetManyShortAsync(request.RequestParameters,
                                                             cancellationToken,
                                                             true);
                                                             
    // Если данные отсутствуют, возвращаем пустой объект.
    if (!sprVlans.Any())
      return new PagedList<SPRVlanDTO>([],0,0,0);

    // Получаем общее количество записей для указанного запроса.
    var count = await locBillUnitOfWork.SPRVlans
                                       .GetCountAsync(request.RequestParameters,
                                                      cancellationToken);

    // Преобразуем данные SPR VLAN в формат DTO.
    var result = mapper.Map<List<SPRVlanDTO>>(sprVlans);

    // Возвращаем объект с данными и информацией о пагинации.
    return new PagedList<SPRVlanDTO>(result,
                                     count,
                                     request.RequestParameters
                                            .PageNumber,
                                     request.RequestParameters
                                            .PageSize);
  }
}

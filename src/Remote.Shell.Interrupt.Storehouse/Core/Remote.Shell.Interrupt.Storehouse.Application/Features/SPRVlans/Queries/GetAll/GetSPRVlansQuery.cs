namespace Remote.Shell.Interrupt.Storehouse.Application.Features.SPRVlans.Queries.GetAll;

// О, черт, запрос на SPR Vlans... Кто вообще придумал эти запросы?!
public record GetSPRVlansQuery(RequestParameters RequestParameters) 
  : IQuery<PagedList<SPRVlanDTO>>;

internal class GetSPRVlansQueryHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                       IMapper mapper) 
  : IQueryHandler<GetSPRVlansQuery, PagedList<SPRVlanDTO>>
{
  async Task<PagedList<SPRVlanDTO>> IRequestHandler<GetSPRVlansQuery, PagedList<SPRVlanDTO>>.Handle(GetSPRVlansQuery request,
                                                                                                    CancellationToken cancellationToken)
  {
    // Загружаем SPR Vlans... Если это провал, я тут ни при чем!
    var sprVlans = await locBillUnitOfWork.SPRVlans
                                          .GetManyShortAsync(request.RequestParameters,
                                                             cancellationToken,
                                                             true);
                                                             
    // Если ничего нет, просто возвращаем пустой список. Легко и просто!
    if (!sprVlans.Any())
      return new PagedList<SPRVlanDTO>([],0,0,0);

    // Получаем количество... Да, это важно. Не спрашивай почему!
    var count = await locBillUnitOfWork.SPRVlans
                                       .GetCountAsync(request.RequestParameters,
                                                      cancellationToken);

    // Преобразуем SPRVlans в DTO. Надеюсь, ты не забудешь про это!
    var result = mapper.Map<List<SPRVlanDTO>>(sprVlans);

    // Возвращаем окончательный результат. Если что-то пошло не так—притворись, что ты меня не видел!
    return new PagedList<SPRVlanDTO>(result,
                                     count,
                                     request.RequestParameters
                                            .PageNumber,
                                     request.RequestParameters
                                            .PageSize);
  }
}

namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetByIdClient;

public record GetClientByIdClientQuery(int IdClient) : IRequest<DetailClientDTO>;

internal class GetClientByIdClientQueryHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                               IMapper mapper) 
    : IRequestHandler<GetClientByIdClientQuery, DetailClientDTO>
{

  async Task<DetailClientDTO> IRequestHandler<GetClientByIdClientQuery, DetailClientDTO>.Handle(GetClientByIdClientQuery request,
                                                                                                CancellationToken cancellationToken)
  {
    var requestParameters = new RequestParameters
    {
      Filters = $"IdClient=={request.IdClient}"
    };

    // Проверка существования клиент с ID
    var existingClient = await locBillUnitOfWork.Clients
                                                .AnyByQueryAsync(requestParameters,
                                                                 cancellationToken);

    // Если клиент не найдено — исключение
    if (!existingClient)
    throw new EntityNotFoundById(typeof(Client),
                                 request.IdClient.ToString());

    var client = await locBillUnitOfWork.Clients
                                        .GetOneWithChildrensAsync(requestParameters,
                                                                  cancellationToken)
        ?? throw new EntityNotFoundException($"Id = {request.IdClient}");
    
    var result = mapper.Map<DetailClientDTO>(client);
    
    return result;
  }
}
namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetById;

public record GetClientByIdQuery(Guid Id) : IRequest<DetailClientDTO>;

internal class GetClientByIdQueryHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                         IMapper mapper) 
    : IRequestHandler<GetClientByIdQuery, DetailClientDTO>
{

    async Task<DetailClientDTO> IRequestHandler<GetClientByIdQuery, DetailClientDTO>.Handle(GetClientByIdQuery request,
                                                                                            CancellationToken cancellationToken)
    {
        var requestParameters = new RequestParameters
        {
        Filters = $"Id=={request.Id}"
        };

        // Проверка существования клиент с ID
        var existingClient = await locBillUnitOfWork.Clients
                                                    .AnyByQueryAsync(requestParameters,
                                                                     cancellationToken);

        // Если клиент не найдено — исключение
        if (!existingClient)
        throw new EntityNotFoundById(typeof(Client),
                                     request.Id.ToString());

        var client = await locBillUnitOfWork.Clients
                                            .GetOneWithChildrensAsync(requestParameters,
                                                                      cancellationToken)
            ?? throw new EntityNotFoundException($"Id = {request.Id}");
        
        var result = mapper.Map<DetailClientDTO>(client);
        
        return result;
    }
}

namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetById;

public record GetClientByIdQuery(Guid Id) : IRequest<DetailClientDTO>;

internal class GetClientByIdQueryHandler(IUnitOfWork unitOfWork,
                                         IMapper mapper) 
    : IRequestHandler<GetClientByIdQuery, DetailClientDTO>
{
    readonly IUnitOfWork _unitOfWork = unitOfWork
        ?? throw new ArgumentNullException(nameof(unitOfWork));
    readonly IMapper _mapper = mapper
        ?? throw new ArgumentNullException(nameof(mapper));

    async Task<DetailClientDTO> IRequestHandler<GetClientByIdQuery, DetailClientDTO>.Handle(GetClientByIdQuery request,
                                                                                            CancellationToken cancellationToken)
    {
        // Проверка существования клиент с ID
        var existingClient = await _unitOfWork.Clients
                                              .AnyByIdAsync(request.Id,
                                                            cancellationToken);

        // Если клиент не найдено — исключение
        if (!existingClient)
        throw new EntityNotFoundById(typeof(Client),
                                    request.Id.ToString());

        var client = await _unitOfWork.Clients
                                      .GetClientWithChildrensByQueryAsync(new RequestParameters()
                                                                          {
                                                                            Filters = $"Id=={request.Id}"
                                                                          },
                                                                          cancellationToken)
            ?? throw new EntityNotFoundException($"Id = {request.Id}");
        
        var result = _mapper.Map<DetailClientDTO>(client);
        
        return result;
    }
}

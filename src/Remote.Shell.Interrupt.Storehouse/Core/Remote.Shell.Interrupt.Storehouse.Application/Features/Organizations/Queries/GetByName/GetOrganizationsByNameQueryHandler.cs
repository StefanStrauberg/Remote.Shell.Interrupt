namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetByName;

public record GetClientsCODByNameQuery(string Name) : IQuery<IEnumerable<ClientCODDTO>>;

internal class GetClientsCODByNameQueryHandler(IUnitOfWork unitOfWork,
                                               IMapper mapper)
  : IQueryHandler<GetClientsCODByNameQuery, IEnumerable<ClientCODDTO>>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<IEnumerable<ClientCODDTO>> IRequestHandler<GetClientsCODByNameQuery, IEnumerable<ClientCODDTO>>.Handle(GetClientsCODByNameQuery request,
                                                                                                                    CancellationToken cancellationToken)
  {
    var clients = await _unitOfWork.ClientCODLs
                                   .GetAllByNameAsync(request.Name,
                                                      cancellationToken);

    if (!clients.Any())
      throw new EntityNotFoundException($"Name = {request.Name}");

    var result = _mapper.Map<IEnumerable<ClientCODDTO>>(clients);

    return result;
  }
}

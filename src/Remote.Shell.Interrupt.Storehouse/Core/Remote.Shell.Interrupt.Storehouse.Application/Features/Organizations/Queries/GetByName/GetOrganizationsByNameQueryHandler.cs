namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetByName;

public record GetOrganizationByNameQuery(string Name) : IQuery<IEnumerable<ClientCODDTO>>;

internal class GetOrganizationsByNameQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
  : IQueryHandler<GetOrganizationByNameQuery, IEnumerable<ClientCODDTO>>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<IEnumerable<ClientCODDTO>> IRequestHandler<GetOrganizationByNameQuery, IEnumerable<ClientCODDTO>>.Handle(GetOrganizationByNameQuery request,
                                                                                                                      CancellationToken cancellationToken)
  {
    var clients = await _unitOfWork.Clients
                                   .GetAllByNameAsync(request.Name,
                                                      cancellationToken);

    if (!clients.Any())
      throw new EntityNotFoundException($"Name = {request.Name}");

    var result = _mapper.Map<IEnumerable<ClientCODDTO>>(clients);

    return result;
  }
}

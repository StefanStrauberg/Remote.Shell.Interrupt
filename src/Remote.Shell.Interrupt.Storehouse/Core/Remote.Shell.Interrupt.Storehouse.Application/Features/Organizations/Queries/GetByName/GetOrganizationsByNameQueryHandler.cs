namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetByName;

public record GetOrganizationByNameQuery(string Name) : IQuery<ClientCODDTO>;

internal class GetOrganizationsByNameQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
  : IQueryHandler<GetOrganizationByNameQuery, ClientCODDTO>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<ClientCODDTO> IRequestHandler<GetOrganizationByNameQuery, ClientCODDTO>.Handle(GetOrganizationByNameQuery request,
                                                                                            CancellationToken cancellationToken)
  {
    var clients = await _unitOfWork.Clients
                                   .GetAllByNameAsync(request.Name,
                                                      cancellationToken);

    var client = clients.FirstOrDefault(x => !string.IsNullOrEmpty(x.VLANTags));

    if (client is null)
      throw new EntityNotFoundException($"Name = {request.Name}");

    var result = _mapper.Map<ClientCODDTO>(client);

    return result;
  }
}

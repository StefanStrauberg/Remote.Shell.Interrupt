namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetAll;

public record GetOrganizationsQuery() : IQuery<IEnumerable<ClientCODDTO>>;

internal class GetOrganizationsQueryHandler(IUnitOfWork unitOfWork,
                                            IMapper mapper)
  : IQueryHandler<GetOrganizationsQuery, IEnumerable<ClientCODDTO>>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<IEnumerable<ClientCODDTO>> IRequestHandler<GetOrganizationsQuery, IEnumerable<ClientCODDTO>>.Handle(GetOrganizationsQuery request,
                                                                                                                 CancellationToken cancellationToken)
  {
    var clients = await _unitOfWork.Clients
                                   .GetAllAsync(cancellationToken);

    var result = _mapper.Map<IEnumerable<ClientCODDTO>>(clients);

    return result;
  }
}

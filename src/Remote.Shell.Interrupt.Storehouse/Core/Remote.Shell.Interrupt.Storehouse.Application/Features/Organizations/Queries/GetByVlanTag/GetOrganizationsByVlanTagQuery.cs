namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetByVlanTag;

public record GetOrganizationByVlanTagQuery(int VlanTag) : IQuery<ClientCODDTO>;

internal class GetOrganizationsByVlanTagQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
  : IQueryHandler<GetOrganizationByVlanTagQuery, ClientCODDTO>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<ClientCODDTO> IRequestHandler<GetOrganizationByVlanTagQuery, ClientCODDTO>.Handle(GetOrganizationByVlanTagQuery request,
                                                                                                CancellationToken cancellationToken)
  {
    var clientName = await _unitOfWork.Clients
                                      .GetClientNameByVlanTagAsync(request.VlanTag,
                                                                   cancellationToken);

    if (clientName is null)
      throw new EntityNotFoundException($"VlanTag = {request.VlanTag}");

    var client = await _unitOfWork.Clients
                                  .GetByNameAsync(clientName,
                                                  cancellationToken);

    var result = _mapper.Map<ClientCODDTO>(client);

    return result;
  }
}
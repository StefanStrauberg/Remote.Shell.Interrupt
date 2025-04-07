namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetByVlanTag;

public record GetClientByVlanTagQuery(int VlanTag) : IQuery<DetailClientDTO>;

internal class GetClientByVlanTagQueryHandler(IUnitOfWork unitOfWork,
                                               IMapper mapper)
  : IQueryHandler<GetClientByVlanTagQuery, DetailClientDTO>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<DetailClientDTO> IRequestHandler<GetClientByVlanTagQuery, DetailClientDTO>.Handle(GetClientByVlanTagQuery request,
                                                                                               CancellationToken cancellationToken)
  {
    // Проверка существования влан с ID
    var existingVlan = await _unitOfWork.SPRVlans
                                        .AnyByVlanTagAsync(request.VlanTag,
                                                           cancellationToken);

    // Если влан не найдено — исключение
    if (!existingVlan)
      throw new EntityNotFoundById(typeof(SPRVlan),
                                   request.VlanTag.ToString());

    var sprVlans = await _unitOfWork.SPRVlans
                                    .GetSPRVlansByQueryAsync(new RequestParameters()
                                                             {
                                                                Filters = $"IdVlan=={request.VlanTag}"
                                                             },
                                                             cancellationToken);

    var clients = await _unitOfWork.Clients
                                   .GetClientsWithChildrensByQueryAsync(new RequestParameters()
                                                                        {
                                                                          Filters = $"IdClient=={sprVlans.First().IdClient}"
                                                                        },
                                                                        cancellationToken);

    return _mapper.Map<DetailClientDTO>(clients.First());
  }
}
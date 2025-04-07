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

    var sprVlan = await _unitOfWork.SPRVlans
                                   .GetSPRVlanByQueryAsync(new RequestParameters()
                                                           {
                                                              Filters = $"IdVlan=={request.VlanTag}"
                                                           },
                                                           cancellationToken);

    var client = await _unitOfWork.Clients
                                  .GetClientWithChildrensByQueryAsync(new RequestParameters()
                                                                      {
                                                                        Filters = $"IdClient=={sprVlan.IdClient}"
                                                                      },
                                                                      cancellationToken);

    return _mapper.Map<DetailClientDTO>(client);
  }
}
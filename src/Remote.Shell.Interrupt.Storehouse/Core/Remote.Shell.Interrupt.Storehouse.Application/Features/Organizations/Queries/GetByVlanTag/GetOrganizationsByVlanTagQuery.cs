namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Queries.GetByVlanTag;

public record GetClientsCODByVlanTagQuery(int VlanTag) : IQuery<IEnumerable<ClientCODDTO>>;

internal class GetClientsCODByVlanTagQueryHandler(IUnitOfWork unitOfWork,
                                                  IMapper mapper)
  : IQueryHandler<GetClientsCODByVlanTagQuery, IEnumerable<ClientCODDTO>>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<IEnumerable<ClientCODDTO>> IRequestHandler<GetClientsCODByVlanTagQuery, IEnumerable<ClientCODDTO>>.Handle(GetClientsCODByVlanTagQuery request,
                                                                                                                       CancellationToken cancellationToken)
  {
    var clientName = await _unitOfWork.ClientCODs
                                      .GetClientNameByVlanTagAsync(request.VlanTag,
                                                                   cancellationToken);

    if (clientName is null)
      throw new EntityNotFoundException($"VlanTag = {request.VlanTag}");

    var name = ExtractNameInQuotes(clientName.TrimEnd());

    var clients = await _unitOfWork.ClientCODs
                                   .GetAllByNameAsync(name,
                                                      cancellationToken);

    var result = _mapper.Map<IEnumerable<ClientCODDTO>>(clients);

    return result;
  }

  static string ExtractNameInQuotes(string input)
  {
    var regex = new Regex(@"^(.*?)\s*\(");
    var match = regex.Match(input);

    if (match.Success)
    {
      return match.Groups[1].Value;
    }

    return input;
  }
}
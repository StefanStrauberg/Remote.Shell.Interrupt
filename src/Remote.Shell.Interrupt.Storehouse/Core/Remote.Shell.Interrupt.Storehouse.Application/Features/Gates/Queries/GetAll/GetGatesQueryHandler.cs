namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Queries.GetAll;

public record GetGatesQuery() : IQuery<IEnumerable<GateDTO>>;

internal class GetGatesQueryHandler(IUnitOfWork unitOfWork,
                                    IMapper mapper)
  : IQueryHandler<GetGatesQuery, IEnumerable<GateDTO>>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<IEnumerable<GateDTO>> IRequestHandler<GetGatesQuery, IEnumerable<GateDTO>>.Handle(GetGatesQuery request,
                                                                                               CancellationToken cancellationToken)
  {
    var gates = await _unitOfWork.GateRepository
                                 .GetAllAsync(cancellationToken);

    var result = _mapper.Map<IEnumerable<GateDTO>>(gates);

    return result;
  }
}

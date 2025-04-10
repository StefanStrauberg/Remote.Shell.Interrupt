namespace Remote.Shell.Interrupt.Storehouse.Application.Features.TfPlans.Queries.GetAll;

public record GetTfPlansQuery(RequestParameters RequestParameters) 
    : IQuery<PagedList<TfPlanDTO>>;

internal class GetTfPlansQueryHandler(IUnitOfWork unitOfWork,
                                      IMapper mapper) 
    : IQueryHandler<GetTfPlansQuery, PagedList<TfPlanDTO>>
{
    readonly IUnitOfWork _unitOfWork = unitOfWork
        ?? throw new ArgumentNullException(nameof(unitOfWork));
    readonly IMapper _mapper = mapper
        ?? throw new ArgumentNullException(nameof(mapper));

    async Task<PagedList<TfPlanDTO>> IRequestHandler<GetTfPlansQuery, PagedList<TfPlanDTO>>.Handle(GetTfPlansQuery request,
                                                                                                   CancellationToken cancellationToken)
    {
        var tfPlans = await _unitOfWork.TfPlans
                                       .GetAllTfPlansAsync(request.RequestParameters,
                                                           cancellationToken);

        if (!tfPlans.Any())
            return new PagedList<TfPlanDTO>([],0,0,0);

        var count = await _unitOfWork.TfPlans
                                     .GetCountAsync(request.RequestParameters,
                                                    cancellationToken);
        var result = _mapper.Map<IEnumerable<TfPlanDTO>>(tfPlans);

        return new PagedList<TfPlanDTO>(result,
                                        count,
                                        request.RequestParameters
                                                .PageNumber,
                                        request.RequestParameters
                                                .PageSize);
    }

}

namespace Remote.Shell.Interrupt.Storehouse.Application.Features.TfPlans.Queries.GetAll;

public record GetTfPlansQuery(RequestParameters RequestParameters) 
    : IQuery<PagedList<TfPlanDTO>>;

internal class GetTfPlansQueryHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                      IMapper mapper) 
    : IQueryHandler<GetTfPlansQuery, PagedList<TfPlanDTO>>
{
    async Task<PagedList<TfPlanDTO>> IRequestHandler<GetTfPlansQuery, PagedList<TfPlanDTO>>.Handle(GetTfPlansQuery request,
                                                                                                   CancellationToken cancellationToken)
    {
        var tfPlans = await locBillUnitOfWork.TfPlans
                                             .GetManyShortAsync(request.RequestParameters,
                                                                cancellationToken);

        if (!tfPlans.Any())
            return new PagedList<TfPlanDTO>([],0,0,0);

        var count = await locBillUnitOfWork.TfPlans
                                           .GetCountAsync(request.RequestParameters,
                                                          cancellationToken);
                                                          
        var result = mapper.Map<IEnumerable<TfPlanDTO>>(tfPlans);

        return new PagedList<TfPlanDTO>(result,
                                        count,
                                        request.RequestParameters
                                                .PageNumber,
                                        request.RequestParameters
                                                .PageSize);
    }

}

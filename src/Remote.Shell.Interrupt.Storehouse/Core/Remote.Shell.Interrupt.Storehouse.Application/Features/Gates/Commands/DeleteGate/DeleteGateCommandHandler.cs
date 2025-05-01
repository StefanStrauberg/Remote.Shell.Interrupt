namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Commands.DeleteGate;

public record DeleteGateCommand(Guid Id) : ICommand<Unit>;

internal class DeleteGateCommandHandler(IGateUnitOfWork gateUnitOfWork,
                                        IGateSpecification specification,
                                        IQueryFilterParser queryFilterParser)
: ICommandHandler<DeleteGateCommand, Unit>
{
  async Task<Unit> IRequestHandler<DeleteGateCommand, Unit>.Handle(DeleteGateCommand request,
                                                                   CancellationToken cancellationToken)
  {
    var requestParameters = new RequestParameters
    {
      Filters = [
        new ()
        {
          PropertyPath = "Id",
          Operator = FilterOperator.Equals,
          Value = request.Id.ToString()
        }
      ]
    };

    // Parse filter
    var filterExpr = queryFilterParser.ParseFilters<Gate>(requestParameters.Filters);

    // Build base specification
    var baseSpec = BuildSpecification(specification,
                                      filterExpr);

    // Проверка существования шлюза с ID
    var existing = await gateUnitOfWork.Gates
                                       .AnyByQueryAsync(baseSpec,
                                                        cancellationToken);
    
    // Если шлюз не найден — исключение
    if (!existing)
      throw new EntityNotFoundException(typeof(Gate),
                                        filterExpr is not null ? filterExpr.ToString() : string.Empty);

    var gate = await gateUnitOfWork.Gates
                                   .GetOneShortAsync(baseSpec,
                                                     cancellationToken);

    gateUnitOfWork.Gates
                  .DeleteOne(gate);

    gateUnitOfWork.Complete();

    return Unit.Value;
  }

  static IGateSpecification BuildSpecification(IGateSpecification baseSpec,
                                               Expression<Func<Gate, bool>>? filterExpr)
  {
    var spec = baseSpec;

    if (filterExpr is not null)
        spec.AddFilter(filterExpr);

    return spec;
  }
}

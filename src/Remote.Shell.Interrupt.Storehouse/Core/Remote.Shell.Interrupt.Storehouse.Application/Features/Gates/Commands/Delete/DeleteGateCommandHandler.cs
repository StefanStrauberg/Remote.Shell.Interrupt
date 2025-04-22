namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Commands.Delete;

public record DeleteGateCommand(Guid Id) : ICommand<Unit>;

internal class DeleteGateCommandHandler(IGateUnitOfWork gateUnitOfWork)
: ICommandHandler<DeleteGateCommand, Unit>
{
  async Task<Unit> IRequestHandler<DeleteGateCommand, Unit>.Handle(DeleteGateCommand request,
                                                                   CancellationToken cancellationToken)
  {
    var requestParameters = new RequestParameters
    {
      Filters = $"Id=={request.Id}"
    };

    var existingGate = await gateUnitOfWork.GateRepository
                                           .AnyByQueryAsync(requestParameters,
                                                            cancellationToken);

    if (!existingGate)
      throw new EntityNotFoundById(typeof(Gate),
                                   request.Id.ToString());

    var gate = await gateUnitOfWork.GateRepository
                                   .GetOneShortAsync(requestParameters,
                                                     cancellationToken);

    gateUnitOfWork.GateRepository
                  .DeleteOne(gate);

    gateUnitOfWork.Complete();

    return Unit.Value;
  }
}

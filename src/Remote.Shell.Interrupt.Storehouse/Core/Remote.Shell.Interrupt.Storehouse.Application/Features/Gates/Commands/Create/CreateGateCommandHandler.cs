namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Commands.Create;

public record CreateGateCommand(CreateGateDTO CreateGateDTO) : ICommand<Unit>;

internal class CreateGateCommandHandler(IGateUnitOfWork gateUnitOfWork,
                                        IMapper mapper)
  : ICommandHandler<CreateGateCommand, Unit>
{
  public async Task<Unit> Handle(CreateGateCommand request,
                                 CancellationToken cancellationToken)
  {
    var requestParameters = new RequestParameters
    {
      Filters = $"IPAddress=={request.CreateGateDTO.IPAddress}"
    };
    
    var existingTheSameIPAddress = await gateUnitOfWork.GateRepository
                                                       .AnyByQueryAsync(requestParameters,
                                                                        cancellationToken);

    if (existingTheSameIPAddress)
      throw new EntityAlreadyExists($"IPAddress = {request.CreateGateDTO.IPAddress}");

    var gate = mapper.Map<Gate>(request.CreateGateDTO);

    gateUnitOfWork.GateRepository
                  .InsertOne(gate);

    gateUnitOfWork.Complete();

    return Unit.Value;
  }
}

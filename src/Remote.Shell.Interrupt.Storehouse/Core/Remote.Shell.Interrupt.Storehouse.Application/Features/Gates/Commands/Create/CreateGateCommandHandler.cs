namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Commands.Create;

public record CreateGateCommand(CreateGateDTO CreateGateDTO) : ICommand<Unit>;

internal class CreateGateCommandHandler(IUnitOfWork unitOfWork,
                                        IMapper mapper)
  : ICommandHandler<CreateGateCommand, Unit>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  public async Task<Unit> Handle(CreateGateCommand request,
                                 CancellationToken cancellationToken)
  {
    var existingTheSameIPAddress = await _unitOfWork.GateRepository
                                                    .AnyByIPAddressAsync(request.CreateGateDTO.IPAddress,
                                                                         cancellationToken);

    if (existingTheSameIPAddress)
      throw new EntityAlreadyExists($"IPAddress = {request.CreateGateDTO.IPAddress}");

    var gate = _mapper.Map<Gate>(request.CreateGateDTO);

    _unitOfWork.GateRepository
               .InsertOne(gate);

    _unitOfWork.Complete();

    return Unit.Value;
  }
}

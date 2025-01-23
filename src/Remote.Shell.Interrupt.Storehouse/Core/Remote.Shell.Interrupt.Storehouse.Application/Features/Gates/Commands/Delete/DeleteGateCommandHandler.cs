namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Commands.Delete;

public record DeleteGateCommand(Guid Id) : ICommand<Unit>;

internal class DeleteGateCommandHandler(IUnitOfWork unitOfWork,
                                        IMapper mapper)
: ICommandHandler<DeleteGateCommand, Unit>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<Unit> IRequestHandler<DeleteGateCommand, Unit>.Handle(DeleteGateCommand request,
                                                                   CancellationToken cancellationToken)
  {
    var existingGate = await _unitOfWork.GateRepository
                                        .AnyByIdAsync(request.Id,
                                                      cancellationToken);

    if (!existingGate)
      throw new EntityNotFoundById(typeof(Gate),
                                   request.Id.ToString());

    var gate = await _unitOfWork.GateRepository
                                .FirstByIdAsync(request.Id,
                                                cancellationToken);

    _unitOfWork.GateRepository.DeleteOne(gate);

    _unitOfWork.Complete();

    return Unit.Value;
  }
}

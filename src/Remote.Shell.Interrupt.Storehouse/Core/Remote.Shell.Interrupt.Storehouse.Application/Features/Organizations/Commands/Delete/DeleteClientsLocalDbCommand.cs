namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Commands.Delete;

public record DeleteClientsLocalDbCommand : ICommand<Unit>;

internal class DeleteClientsLocalDbCommandHandler(IUnitOfWork unitOfWork) 
  : ICommandHandler<DeleteClientsLocalDbCommand, Unit>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));

  async Task<Unit> IRequestHandler<DeleteClientsLocalDbCommand, Unit>.Handle(DeleteClientsLocalDbCommand request,
                                                                             CancellationToken cancellationToken)
  {
    var tfPlans = await _unitOfWork.TfPlans
                                   .GetAllAsync(cancellationToken);
    var sprVlans = await _unitOfWork.SPRVlans
                                    .GetAllAsync(cancellationToken);
    var clients = await _unitOfWork.Clients
                                   .GetAllAsync(cancellationToken);
    var cods = await _unitOfWork.CODs
                                .GetAllAsync(cancellationToken);

    if (tfPlans.Any())
      _unitOfWork.TfPlans
                 .DeleteMany(tfPlans);
    
    if (sprVlans.Any())
      _unitOfWork.SPRVlans
                 .DeleteMany(sprVlans);

    if (clients.Any())
      _unitOfWork.Clients
                 .DeleteMany(clients);
    
    if (cods.Any())
      _unitOfWork.CODs
                 .DeleteMany(cods);

    if (!clients.Any() && !sprVlans.Any() && !tfPlans.Any() && !cods.Any())
      return Unit.Value;

    _unitOfWork.Complete();

    return Unit.Value;
  }
}

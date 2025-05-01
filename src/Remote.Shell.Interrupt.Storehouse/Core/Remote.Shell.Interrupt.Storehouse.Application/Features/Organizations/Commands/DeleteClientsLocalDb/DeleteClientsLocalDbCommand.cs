namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Commands.DeleteClientsLocalDb;

public record DeleteClientsLocalDbCommand : ICommand<Unit>;

internal class DeleteClientsLocalDbCommandHandler(ILocBillUnitOfWork locBillUnitOfWork) 
  : ICommandHandler<DeleteClientsLocalDbCommand, Unit>
{
  async Task<Unit> IRequestHandler<DeleteClientsLocalDbCommand, Unit>.Handle(DeleteClientsLocalDbCommand request,
                                                                             CancellationToken cancellationToken)
  {
    var tfPlans = await locBillUnitOfWork.TfPlans
                                         .GetAllAsync(cancellationToken);
    var sprVlans = await locBillUnitOfWork.SPRVlans
                                          .GetAllAsync(cancellationToken);
    var clients = await locBillUnitOfWork.Clients
                                         .GetAllAsync(cancellationToken);
    var cods = await locBillUnitOfWork.CODs
                                      .GetAllAsync(cancellationToken);

    if (tfPlans.Any())
      locBillUnitOfWork.TfPlans
                       .DeleteMany(tfPlans);
    
    if (sprVlans.Any())
      locBillUnitOfWork.SPRVlans
                       .DeleteMany(sprVlans);

    if (clients.Any())
      locBillUnitOfWork.Clients
                       .DeleteMany(clients);
    
    if (cods.Any())
      locBillUnitOfWork.CODs
                       .DeleteMany(cods);

    if (!clients.Any() && !sprVlans.Any() && !tfPlans.Any() && !cods.Any())
      return Unit.Value;

    locBillUnitOfWork.Complete();

    return Unit.Value;
  }
}

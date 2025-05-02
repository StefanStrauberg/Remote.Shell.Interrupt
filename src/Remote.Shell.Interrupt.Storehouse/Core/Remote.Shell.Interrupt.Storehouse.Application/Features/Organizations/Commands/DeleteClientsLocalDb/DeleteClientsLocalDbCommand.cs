namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Commands.DeleteClientsLocalDb;

/// <summary>
/// Represents a command to delete all clients and related local database records.
/// </summary>
public record DeleteClientsLocalDbCommand : ICommand<Unit>;

/// <summary>
/// Handles the DeleteClientsLocalDbCommand and removes all relevant local database records.
/// </summary>
/// <remarks>
/// This handler retrieves all records from multiple tables, checks if they exist, 
/// deletes them if necessary, and commits the transaction.
/// </remarks>
/// <param name="locBillUnitOfWork">Unit of work for managing database operations.</param>
internal class DeleteClientsLocalDbCommandHandler(ILocBillUnitOfWork locBillUnitOfWork) 
  : ICommandHandler<DeleteClientsLocalDbCommand, Unit>
{
  /// <summary>
  /// Handles the request to delete all local database records for clients.
  /// </summary>
  /// <param name="request">The command initiating the deletion process.</param>
  /// <param name="cancellationToken">Token to handle request cancellation.</param>
  /// <returns>A unit value indicating successful execution.</returns>
  async Task<Unit> IRequestHandler<DeleteClientsLocalDbCommand, Unit>.Handle(DeleteClientsLocalDbCommand request,
                                                                             CancellationToken cancellationToken)
  {
    // Retrieve all TF plans, SPR VLANs, clients, and CODs
    var tfPlans = await locBillUnitOfWork.TfPlans
                                         .GetAllAsync(cancellationToken);
    var sprVlans = await locBillUnitOfWork.SPRVlans
                                          .GetAllAsync(cancellationToken);
    var clients = await locBillUnitOfWork.Clients
                                         .GetAllAsync(cancellationToken);
    var cods = await locBillUnitOfWork.CODs
                                      .GetAllAsync(cancellationToken);

    // Delete records if they exist
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

    // If all records are empty, return immediately
    if (!clients.Any() && !sprVlans.Any() && !tfPlans.Any() && !cods.Any())
      return Unit.Value;

    // Commit the transaction
    locBillUnitOfWork.Complete();

    // Return unit value indicating success
    return Unit.Value;
  }
}

namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Commands.UpdateClientsLocalDb;

/// <summary>
/// Represents a command to update local client database records based on remote data.
/// </summary>
public record UpdateClientsLocalDbCommand : ICommand;

/// <summary>
/// Handles the UpdateClientsLocalDbCommand and synchronizes local client-related data
/// with remote records.
/// </summary>
/// <remarks>
/// This handler removes existing local records, retrieves updated data from the remote database,
/// and inserts new entries to maintain consistency.
/// </remarks>
/// <param name="locBillUnitOfWork">Unit of work for local database operations.</param>
/// <param name="remBillUnitOfWork">Unit of work for remote database operations.</param>
internal class UpdateClientsLocalDbCommandHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                                  IRemBillUnitOfWork remBillUnitOfWork)
  : ICommandHandler<UpdateClientsLocalDbCommand, Unit>
{
  /// <summary>
  /// Handles the request to update local client records based on remote data.
  /// </summary>
  /// <param name="request">The command initiating the update process.</param>
  /// <param name="cancellationToken">Token to handle request cancellation.</param>
  /// <returns>A unit value indicating successful execution.</returns>
  async Task<Unit> IRequestHandler<UpdateClientsLocalDbCommand, Unit>.Handle(UpdateClientsLocalDbCommand request,
                                                                             CancellationToken cancellationToken)
  {
    // Retrieve all remote records
    var allClientCODRs = await remBillUnitOfWork.RemoteClients.GetAllAsync(cancellationToken);
    var allCODRs = await remBillUnitOfWork.RemoteCODs.GetAllAsync(cancellationToken);
    var allTfPlanRs = await remBillUnitOfWork.RemoteTfPlans.GetAllAsync(cancellationToken);
    var allSPRVlanRs = await remBillUnitOfWork.RemoteSPRVlans.GetAllAsync(cancellationToken);

    // Remove existing local records
    var CODLsToDel = await locBillUnitOfWork.CODs.GetAllAsync(cancellationToken);
    if (CODLsToDel.Any())
      locBillUnitOfWork.CODs.DeleteMany(CODLsToDel);

    var TfPlanLsToDel = await locBillUnitOfWork.TfPlans.GetAllAsync(cancellationToken);
    if (TfPlanLsToDel.Any())
      locBillUnitOfWork.TfPlans.DeleteMany(TfPlanLsToDel);

    var ClientCodLstoDel = await locBillUnitOfWork.Clients.GetAllAsync(cancellationToken);
    if (ClientCodLstoDel.Any())
      locBillUnitOfWork.Clients.DeleteMany(ClientCodLstoDel);

    var SPRVlanLsToDel = await locBillUnitOfWork.SPRVlans.GetAllAsync(cancellationToken);
    if (SPRVlanLsToDel.Any())
      locBillUnitOfWork.SPRVlans.DeleteMany(SPRVlanLsToDel);

    // Prepare lists for new records
    List<COD> CODLsToCre = [];
    List<TfPlan> TfPlanLsToCre = [];
    List<Client> ClientCodLsToCre = [];
    List<SPRVlan> SPRVlanLsToCre = [];

    // Map and insert COD records
    foreach (var cod in allCODRs)
    {
      CODLsToCre.Add(new COD
      {
        IdCOD = cod.IdCOD,
        NameCOD = cod.NameCOD.TrimEnd(),
        Telephone = cod.Telephone?.TrimEnd() ?? string.Empty,
        Email1 = cod.Email1?.TrimEnd() ?? string.Empty,
        Email2 = cod.Email2?.TrimEnd() ?? string.Empty,
        Contact = cod.Contact?.TrimEnd() ?? string.Empty,
        Description = cod.Description?.TrimEnd() ?? string.Empty,
        Region = cod.Region?.TrimEnd() ?? string.Empty
      });
    }
    locBillUnitOfWork.CODs.InsertMany(CODLsToCre);

    // Map and insert TfPlan records
    foreach (var tfPlan in allTfPlanRs)
    {
      TfPlanLsToCre.Add(new TfPlan
      {
        IdTfPlan = tfPlan.IdTfPlan,
        NameTfPlan = tfPlan.NameTfPlan.TrimEnd(),
        DescTfPlan = tfPlan.DescTfPlan.TrimEnd().Replace("\0", "")
      });
    }
    locBillUnitOfWork.TfPlans.InsertMany(TfPlanLsToCre);

    // Map and insert Client records
    foreach (var client in allClientCODRs)
    {
      ClientCodLsToCre.Add(new Client
      {
        IdClient = client.IdClient,
        Dat1 = client.Dat1 is null ? null : DateTime.SpecifyKind(client.Dat1.Value, DateTimeKind.Utc),
        Dat2 = client.Dat2 is null ? null : DateTime.SpecifyKind(client.Dat2.Value, DateTimeKind.Utc),
        Prim1 = client.Prim1?.TrimEnd(),
        Prim2 = client.Prim2?.TrimEnd(),
        Nik = client.Nik?.TrimEnd(),
        NrDogovor = client.NrDogovor.TrimEnd(),
        Name = client.Name.TrimEnd(),
        ContactC = client.ContactC.TrimEnd(),
        TelephoneC = client.TelephoneC.TrimEnd(),
        ContactT = client.ContactT.TrimEnd(),
        TelephoneT = client.TelephoneT.TrimEnd(),
        EmailC = client.EmailC.TrimEnd(),
        Working = client.Working,
        EmailT = client.EmailT.TrimEnd(),
        History = client.History.TrimEnd().Replace("\0", ""),
        AntiDDOS = client.AntiDDOS,
        Id_COD = client.Id_COD,
        Id_TfPlan = client.Id_TfPlan,
      });
    }
    locBillUnitOfWork.Clients.InsertMany(ClientCodLsToCre);

    // Map and insert SPRVlan records
    foreach (var SPRVlan in allSPRVlanRs)
    {
      SPRVlanLsToCre.Add(new SPRVlan
      {
        IdVlan = SPRVlan.IdVlan,
        IdClient = SPRVlan.IdClient,
        UseClient = SPRVlan.UseClient,
        UseCOD = SPRVlan.UseCOD
      });
    }
    locBillUnitOfWork.SPRVlans.InsertMany(SPRVlanLsToCre);

    // Commit the transaction
    locBillUnitOfWork.Complete();

    // Return a unit value indicating completion
    return Unit.Value;
  }
}

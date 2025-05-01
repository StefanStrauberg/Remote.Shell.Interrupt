namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Commands.UpdateClientsLocalDb;

public record UpdateClientsLocalDbCommand : ICommand;

internal class UpdateClientsLocalDbCommandHandler(ILocBillUnitOfWork locBillUnitOfWork,
                                                  IRemBillUnitOfWork remBillUnitOfWork)
  : ICommandHandler<UpdateClientsLocalDbCommand, Unit>
{
  async Task<Unit> IRequestHandler<UpdateClientsLocalDbCommand, Unit>.Handle(UpdateClientsLocalDbCommand request,
                                                                             CancellationToken cancellationToken)
  {
    var allClientCODRs = await remBillUnitOfWork.RemoteClients.GetAllAsync(cancellationToken);
    var allCODRs = await remBillUnitOfWork.RemoteCODs.GetAllAsync(cancellationToken);
    var allTfPlanRs = await remBillUnitOfWork.RemoteTfPlans.GetAllAsync(cancellationToken);
    var allSPRVlanRs = await remBillUnitOfWork.RemoteSPRVlans.GetAllAsync(cancellationToken);

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

    List<COD> CODLsToCre = [];
    List<TfPlan> TfPlanLsToCre = [];
    List<Client> ClientCodLsToCre = [];
    List<SPRVlan> SPRVlanLsToCre = [];

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

    foreach (var client in allClientCODRs)
    {
      ClientCodLsToCre.Add(new Client
      {
        IdClient = client.IdClient,
        Dat1 = client.Dat1,
        Dat2 = client.Dat2,
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

    locBillUnitOfWork.Complete();

    return Unit.Value;
  }
}

namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Commands.Update;

public record UpdateClientsLocalDbCommand : ICommand;

internal class UpdateClientsLocalDbCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  : ICommandHandler<UpdateClientsLocalDbCommand, Unit>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<Unit> IRequestHandler<UpdateClientsLocalDbCommand, Unit>.Handle(UpdateClientsLocalDbCommand request,
                                                                             CancellationToken cancellationToken)
  {
    var allClientCODRs = await _unitOfWork.RemoteClients.GetAllAsync(cancellationToken);
    var allCODRs = await _unitOfWork.RemoteCODs.GetAllAsync(cancellationToken);
    var allTfPlanRs = await _unitOfWork.RemoteTfPlans.GetAllAsync(cancellationToken);
    var allSPRVlanRs = await _unitOfWork.RemoteSPRVlans.GetAllAsync(cancellationToken);

    var CODLsToDel = await _unitOfWork.CODs.GetAllAsync(cancellationToken);
    if (CODLsToDel.Any())
      _unitOfWork.CODs.DeleteMany(CODLsToDel);

    var TfPlanLsToDel = await _unitOfWork.TfPlans.GetAllAsync(cancellationToken);
    if (TfPlanLsToDel.Any())
      _unitOfWork.TfPlans.DeleteMany(TfPlanLsToDel);

    var ClientCodLstoDel = await _unitOfWork.Clients.GetAllAsync(cancellationToken);
    if (ClientCodLstoDel.Any())
      _unitOfWork.Clients.DeleteMany(ClientCodLstoDel);

    var SPRVlanLsToDel = await _unitOfWork.SPRVlans.GetAllAsync(cancellationToken);
    if (SPRVlanLsToDel.Any())
      _unitOfWork.SPRVlans.DeleteMany(SPRVlanLsToDel);

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

    _unitOfWork.CODs.InsertMany(CODLsToCre);

    foreach (var tfPlan in allTfPlanRs)
    {
      TfPlanLsToCre.Add(new TfPlan
      {
        IdTfPlan = tfPlan.IdTfPlan,
        NameTfPlan = tfPlan.NameTfPlan.TrimEnd(),
        DescTfPlan = tfPlan.DescTfPlan.TrimEnd().Replace("\0", "")
      });
    }

    _unitOfWork.TfPlans.InsertMany(TfPlanLsToCre);

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

    _unitOfWork.Clients.InsertMany(ClientCodLsToCre);

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

    _unitOfWork.SPRVlans.InsertMany(SPRVlanLsToCre);

    _unitOfWork.Complete();

    return Unit.Value;
  }
}

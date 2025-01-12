namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Organizations.Commands.Update;

public record UpdateOrganizationsLocalDbCommand : ICommand;

internal class UpdateOrganizationLocalDbCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
  : ICommandHandler<UpdateOrganizationsLocalDbCommand, Unit>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<Unit> IRequestHandler<UpdateOrganizationsLocalDbCommand, Unit>.Handle(UpdateOrganizationsLocalDbCommand request,
                                                                                   CancellationToken cancellationToken)
  {
    var allClientCODRs = await _unitOfWork.ClientCODRs.GetAllAsync(cancellationToken);
    var allCODRs = await _unitOfWork.CODRs.GetAllAsync(cancellationToken);
    var allTfPlanRs = await _unitOfWork.TfPlanRs.GetAllAsync(cancellationToken);
    var allSPRVlanRs = await _unitOfWork.SPRVlanRs.GetAllAsync(cancellationToken);

    var CODLsToDel = await _unitOfWork.CODLs.GetAllAsync(cancellationToken);
    if (CODLsToDel.Any())
      _unitOfWork.CODLs.DeleteMany(CODLsToDel);

    var TfPlanLsToDel = await _unitOfWork.TfPlanLs.GetAllAsync(cancellationToken);
    if (TfPlanLsToDel.Any())
      _unitOfWork.TfPlanLs.DeleteMany(TfPlanLsToDel);

    var ClientCodLstoDel = await _unitOfWork.ClientCODLs.GetAllAsync(cancellationToken);
    if (ClientCodLstoDel.Any())
      _unitOfWork.ClientCODLs.DeleteMany(ClientCodLstoDel);

    var SPRVlanLsToDel = await _unitOfWork.SPRVlanLs.GetAllAsync(cancellationToken);
    if (SPRVlanLsToDel.Any())
      _unitOfWork.SPRVlanLs.DeleteMany(SPRVlanLsToDel);

    List<CODL> CODLsToCre = [];
    List<TfPlanL> TfPlanLsToCre = [];
    List<ClientCODL> ClientCodLsToCre = [];
    List<SPRVlanL> SPRVlanLsToCre = [];

    foreach (var cod in allCODRs)
    {
      CODLsToCre.Add(new CODL
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

    _unitOfWork.CODLs.InsertMany(CODLsToCre);

    foreach (var tfPlan in allTfPlanRs)
    {
      TfPlanLsToCre.Add(new TfPlanL
      {
        IdTfPlan = tfPlan.Id,
        NameTfPlan = tfPlan.NameTfPlan.TrimEnd(),
        DescTfPlan = tfPlan.DescTfPlan.TrimEnd().Replace("\0", "")
      });
    }

    _unitOfWork.TfPlanLs.InsertMany(TfPlanLsToCre);

    foreach (var client in allClientCODRs)
    {
      ClientCodLsToCre.Add(new ClientCODL
      {
        IdClient = client.Id,
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
        IdCOD = CODLsToCre.Where(x => x.IdCOD == client.IdCOD)
                          .Select(x => x.Id)
                          .First(),
        COD = CODLsToCre.Where(x => x.IdCOD == client.IdCOD)
                        .First(),
        IdTPlan = TfPlanLsToCre.Where(x => x.IdTfPlan == client.IdTfPlan)
                          .Select(x => x.Id)
                          .FirstOrDefault(),
        TfPlanL = TfPlanLsToCre.Where(x => x.IdTfPlan == client.IdTfPlan)
                               .First(),
      });
    }

    _unitOfWork.ClientCODLs.InsertMany(ClientCodLsToCre);

    foreach (var SPRVlan in allSPRVlanRs)
    {
      SPRVlanLsToCre.Add(new SPRVlanL
      {
        IdVlan = SPRVlan.IdVlan,
        IdClient = ClientCodLsToCre.Where(x => x.IdClient == SPRVlan.IdClient)
                                   .Select(x => x.Id)
                                   .FirstOrDefault(),
        UseClient = SPRVlan.UseClient,
        UseCOD = SPRVlan.UseCOD
      });
    }

    _unitOfWork.SPRVlanLs.InsertMany(SPRVlanLsToCre);

    _unitOfWork.Complete();

    return Unit.Value;
  }
}

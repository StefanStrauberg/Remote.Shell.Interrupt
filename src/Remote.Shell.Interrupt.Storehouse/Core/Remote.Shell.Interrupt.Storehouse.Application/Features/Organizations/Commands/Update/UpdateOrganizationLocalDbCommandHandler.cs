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

    var CODLsToDel = await _unitOfWork.CODLs.GetAllAsync(cancellationToken);
    if (CODLsToDel.Any())
      _unitOfWork.CODLs.DeleteMany(CODLsToDel);

    var TfPlanLsToDel = await _unitOfWork.TfPlanLs.GetAllAsync(cancellationToken);
    if (TfPlanLsToDel.Any())
      _unitOfWork.TfPlanLs.DeleteMany(TfPlanLsToDel);

    var ClientCodLstoDel = await _unitOfWork.ClientCodLs.GetAllAsync(cancellationToken);
    if (ClientCodLstoDel.Any())
      _unitOfWork.ClientCodLs.DeleteMany(ClientCodLstoDel);

    List<CODL> CODLsToCre = [];
    List<TfPlanL> TfPlanLs = [];
    List<ClientCodL> ClientCodLsToCre = [];

    foreach (var cod in allCODRs)
    {
      CODLsToCre.Add(new CODL
      {
        IdCOD = cod.Id,
        NameCOD = cod.NameCOD.TrimEnd(),
        Telephone = cod.Telephone.TrimEnd(),
        Email1 = cod.Email1.TrimEnd(),
        Email2 = cod.Email2.TrimEnd(),
        Contact = cod.Contact.TrimEnd(),
        Description = cod.Description.TrimEnd(),
        Region = cod.Region.TrimEnd()
      });
    }

    _unitOfWork.CODLs.InsertMany(CODLsToCre);

    foreach (var tfPlan in allTfPlanRs)
    {
      TfPlanLs.Add(new TfPlanL
      {
        IdTfPlan = tfPlan.Id,
        NameTfPlan = tfPlan.NameTfPlan.TrimEnd(),
        DescTfPlan = tfPlan.DescTfPlan.TrimEnd().Replace("\0", "")
      });
    }

    _unitOfWork.TfPlanLs.InsertMany(TfPlanLs);

    foreach (var client in allClientCODRs)
    {
      ClientCodLsToCre.Add(new ClientCodL
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
        COD = client.COD,
        IdTPlan = TfPlanLs.Where(x => x.IdTfPlan == client.IdTfPlan)
                          .Select(x => x.Id)
                          .FirstOrDefault(),
        TfPlanL = client.TfPlan,
      });
    }
    _unitOfWork.ClientCodLs.InsertMany(ClientCodLsToCre);

    _unitOfWork.Complete();

    return Unit.Value;
  }
}

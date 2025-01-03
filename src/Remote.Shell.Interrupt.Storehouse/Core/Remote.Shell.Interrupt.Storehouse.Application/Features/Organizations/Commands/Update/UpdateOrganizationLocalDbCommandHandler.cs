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
    var allClients = await _unitOfWork.ClientCODRs.GetAllAsync(cancellationToken);
    var allCODs = await _unitOfWork.CODRs.GetAllAsync(cancellationToken);
    var allTfPlans = await _unitOfWork.TfPlanRs.GetAllAsync(cancellationToken);



    List<ClientCodL> organizations = [];

    foreach (var client in allClients)
    {
      organizations.Add(new ClientCodL
      {
        IdClient = client.Id,
        Name = client.Name,
        ContactC = client.ContactC,
        TelephoneC = client.TelephoneC,
        ContactT = client.ContactT,
        TelephoneT = client.TelephoneT,
        EmailC = client.EmailC,
        Working = client.Working,
        EmailT = client.EmailT,
        History = client.History,
        AntiDDOS = client.AntiDDOS,
        IdCOD = client.IdCOD,
        COD = client.COD,
        IdTPlan = client.IdTfPlan,
        TfPlanL = client.TfPlan,
      });
    }

    var toDelete = await _unitOfWork.Organizations.GetAllAsync(cancellationToken);

    if (toDelete.Any())
      _unitOfWork.Organizations.DeleteMany(toDelete);

    _unitOfWork.Organizations.InsertMany(organizations);
    _unitOfWork.Complete();

    return Unit.Value;
  }
}

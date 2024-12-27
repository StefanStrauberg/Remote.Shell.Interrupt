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
    var allClients = await _unitOfWork.ClientCODs.GetAllAsync(cancellationToken);

    List<Organization> organizations = [];

    foreach (var client in allClients)
    {
      organizations.Add(new Organization
      {
        IdClient = client.IdClient,
        Name = client.Name,
        ContactC = client.ContactT,
        EmailC = client.EmailC,
        IdTPlan = client.IdTPlan
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

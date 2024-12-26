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
    var allClients = await _unitOfWork.Clients.GetAllAsync(cancellationToken);

    List<ClientCod> organizations = [];

    foreach (var client in allClients)
    {
      organizations.Add(new ClientCod
      {
        IdClient = client.Id,
        Name = client.Name,
        ContactC = client.Contact,
        Email = client.Email,
        TPlan = client.TPlan,
        VLANTags = ConvertStringToIntArray(client.VLANTags)
      });
    }

    var toDelete = await _unitOfWork.Organizations.GetAllAsync(cancellationToken);

    if (toDelete.Any())
      _unitOfWork.Organizations.DeleteMany(toDelete);

    _unitOfWork.Organizations.InsertMany(organizations);
    _unitOfWork.Complete();

    return Unit.Value;
  }
  static int[] ConvertStringToIntArray(string input)
  {
    if (string.IsNullOrEmpty(input))
      return [];

    return input.Split(',')
                .Select(x => int.Parse(x.Trim()))
                .ToArray();
  }
}

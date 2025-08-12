namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Gates.Commands.UpdateGate;


public record UpdateGateCommand(UpdateGateDTO UpdateGateDTO) 
  : UpdateEntityCommand<Gate, UpdateGateDTO>(UpdateGateDTO.Id, UpdateGateDTO);

internal class UpdateGateGommandHandler(IGateUnitOfWork gateUnitOfWork,
                                        IGateSpecification specification,
                                        IQueryFilterParser queryFilterParser,
                                        IMapper mapper)
  : UpdateEntityCommandHandler<Gate, UpdateGateDTO, UpdateGateCommand>(specification, queryFilterParser, mapper)
{
  protected override async Task EnsureEntityExistAsync(ISpecification<Gate> specification, CancellationToken cancellationToken)
  {
    bool exists = await gateUnitOfWork.Gates.AnyByQueryAsync(specification, cancellationToken);

    if (exists is not true)
      throw new EntityNotFoundException(typeof(Gate), specification.ToString() ?? string.Empty);
  }

  protected override async Task<Gate> FetchEntityAsync(ISpecification<Gate> specification, CancellationToken cancellationToken)
    => await gateUnitOfWork.Gates.GetOneShortAsync(specification, cancellationToken);

  protected override void UpdateEntity(Gate entity)
  {
    gateUnitOfWork.Gates.ReplaceOne(entity);
    gateUnitOfWork.Complete();
  }
}

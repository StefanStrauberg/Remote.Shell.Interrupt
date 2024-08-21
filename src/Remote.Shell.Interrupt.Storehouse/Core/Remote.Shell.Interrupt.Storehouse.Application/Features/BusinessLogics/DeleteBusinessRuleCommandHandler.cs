namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics;

public record DeleteBusinessRuleCommand(Guid Id) : ICommand
{

}

internal class DeleteBusinessRuleCommandHandler(IBusinessRuleRepository businessRuleRepository)
  : ICommandHandler<DeleteBusinessRuleCommand, Unit>
{
  readonly IBusinessRuleRepository _businessRuleRepository = businessRuleRepository
    ?? throw new ArgumentNullException(nameof(businessRuleRepository));

  async Task<Unit> IRequestHandler<DeleteBusinessRuleCommand, Unit>.Handle(DeleteBusinessRuleCommand request,
                                                                           CancellationToken cancellationToken)
  {
    var existingBusinessRule = await _businessRuleRepository.ExistsAsync(filterExpression: x => x.Id == request.Id,
                                                                         cancellationToken: cancellationToken);

    if (!existingBusinessRule)
      throw new EntityNotFoundException(request.Id.ToString());

    await _businessRuleRepository.DeleteOneAsync(x => x.Id == request.Id,
                                                 cancellationToken);
    return Unit.Value;
  }
}

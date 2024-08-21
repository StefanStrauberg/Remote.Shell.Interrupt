
namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics;

public record UpdateBusinessRuleCommand(Guid Id,
                                        string Name,
                                        Expression<Func<object, bool>>? Condition,
                                        int[] Branch,
                                        int SequenceNumber,
                                        Assignment? Assignment) : ICommand;


internal class UpdateBusinessRuleCommandHandler(IBusinessRuleRepository businessRuleRepository)
  : ICommandHandler<UpdateBusinessRuleCommand, Unit>
{
  readonly IBusinessRuleRepository _businessRuleRepository = businessRuleRepository
    ?? throw new ArgumentNullException(nameof(businessRuleRepository));

  async Task<Unit> IRequestHandler<UpdateBusinessRuleCommand, Unit>.Handle(UpdateBusinessRuleCommand request,
                                                                           CancellationToken cancellationToken)
  {
    var existingUpdatingBusinessRule = await _businessRuleRepository.ExistsAsync(filterExpression: x => x.Id == request.Id &&
                                                                                                   x.Branch == request.Branch &&
                                                                                                   x.SequenceNumber == request.SequenceNumber,
                                                                                 cancellationToken: cancellationToken);

    if (!existingUpdatingBusinessRule)
      throw new EntityNotFoundException(request.Id.ToString());

    var updatingBusinessRule = request.Adapt<BusinessRule>();

    await _businessRuleRepository.ReplaceOneAsync(filterExpression: x => x.Id == request.Id,
                                                  document: updatingBusinessRule,
                                                  cancellationToken: cancellationToken);
    return Unit.Value;
  }
}

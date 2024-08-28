namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics;

public record UpdateBusinessRuleCommand(Guid Id,
                                        UpdateBusinessRuleDTO UpdateBusinessRule)
  : ICommand;

internal class UpdateBusinessRuleCommandHandler(IBusinessRuleRepository businessRuleRepository)
  : ICommandHandler<UpdateBusinessRuleCommand, Unit>
{
  readonly IBusinessRuleRepository _businessRuleRepository = businessRuleRepository
    ?? throw new ArgumentNullException(nameof(businessRuleRepository));

  async Task<Unit> IRequestHandler<UpdateBusinessRuleCommand, Unit>.Handle(UpdateBusinessRuleCommand request,
                                                                           CancellationToken cancellationToken)
  {
    // Looking for by ID
    Expression<Func<BusinessRule, bool>> filter = x => x.Id == request.Id;

    var existingUpdatingBusinessRule = await _businessRuleRepository.ExistsAsync(filterExpression: filter,
                                                                                 cancellationToken: cancellationToken);

    if (!existingUpdatingBusinessRule)
      throw new EntityNotFoundException(request.Id.ToString());

    var updatingBusinessRule = request.Adapt<BusinessRule>();
    var originalBusinessRule = await _businessRuleRepository.FindOneAsync(filterExpression: filter,
                                                                          cancellationToken: cancellationToken);

    updatingBusinessRule.Id = request.Id;
    updatingBusinessRule.Modified = DateTime.UtcNow;
    updatingBusinessRule.ParentId = originalBusinessRule.ParentId;
    updatingBusinessRule.Children = originalBusinessRule.Children;

    await _businessRuleRepository.ReplaceOneAsync(filterExpression: x => x.Id == request.Id,
                                                  document: updatingBusinessRule,
                                                  cancellationToken: cancellationToken);
    return Unit.Value;
  }
}

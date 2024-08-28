namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics;

public record CreateBusinessRuleCommand(CreateBusinessRuleDTO CreateBusinessRuleDTO) : ICommand;

internal class CreateBusinessRuleCommandHandler(IBusinessRuleRepository businessRuleRepository)
  : ICommandHandler<CreateBusinessRuleCommand, Unit>
{
  readonly IBusinessRuleRepository _businessRuleRepository = businessRuleRepository
    ?? throw new ArgumentNullException(nameof(businessRuleRepository));

  async Task<Unit> IRequestHandler<CreateBusinessRuleCommand, Unit>.Handle(CreateBusinessRuleCommand request,
                                                                           CancellationToken cancellationToken)
  {
    var filter = (Expression<Func<BusinessRule, bool>>)(x => x.Name == request.CreateBusinessRuleDTO.Name);
    var existingBusinessRule = await _businessRuleRepository.ExistsAsync(filterExpression: filter,
                                                                         cancellationToken: cancellationToken);

    if (existingBusinessRule)
      throw new EntityAlreadyExists(request.CreateBusinessRuleDTO
                                           .Name);

    var businessRules = await _businessRuleRepository.GetAllAsync(cancellationToken);
    var addingRule = request.CreateBusinessRuleDTO
                            .Adapt<BusinessRule>();

    addingRule.Created = DateTime.UtcNow;
    addingRule.Modified = DateTime.UtcNow;

    // Check if the rule has a parent
    if (request.CreateBusinessRuleDTO.ParentId != null)
    {
      var parentRule = await _businessRuleRepository.FindOneAsync(x => x.Id == request.CreateBusinessRuleDTO.ParentId,
                                                                  cancellationToken)
        ?? throw new ArgumentException("The specified parent rule does not exist.");

      await _businessRuleRepository.InsertOneAsync(document: addingRule,
                                                   cancellationToken: cancellationToken);

      parentRule.Children.Add(addingRule.Id);

      await _businessRuleRepository.ReplaceOneAsync(x => x.Id == parentRule.Id,
                                                    parentRule,
                                                    cancellationToken);
    }
    else
    {
      await _businessRuleRepository.InsertOneAsync(document: addingRule,
                                                   cancellationToken: cancellationToken);
    }

    return Unit.Value;
  }
}

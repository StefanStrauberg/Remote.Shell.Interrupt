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

    var requestBranches = request.CreateBusinessRuleDTO.Branch;

    if (businessRules.Count == 0)
    {
      // This is the first rule, create root branch [1]
      addingRule.Branch = new List<int> { 1 };
      addingRule.SequenceNumber = 0;
    }
    else
    {
      var existingRulesWithSameBranch = businessRules
          .Where(x => x.Branch != null && x.Branch.SequenceEqual(requestBranches))
          .ToList();

      if (existingRulesWithSameBranch.Count > 0)
      {
        // If branch exists, set the sequence number
        addingRule.SequenceNumber = existingRulesWithSameBranch.Max(rule => rule.SequenceNumber) + 1;
      }
      else
      {
        // Branch does not exist, find parent branch
        var parentBranch = requestBranches.Take(requestBranches.Count - 1).ToList();
        var existingRulesWithParentBranch = businessRules
            .Where(x => x.Branch != null && x.Branch.SequenceEqual(parentBranch))
            .ToList();

        if (existingRulesWithParentBranch.Count == 0)
        {
          throw new ArgumentException("The adding business rule has a wrong branch.");
        }
        else
        {
          // Create a new branch sequence for the new rule
          var nextBranchValue = requestBranches.Last();
          addingRule.Branch = new List<int>(parentBranch) { nextBranchValue };
          addingRule.SequenceNumber = 0;
        }
      }
    }

    await _businessRuleRepository.InsertOneAsync(document: addingRule,
                                                 cancellationToken: cancellationToken);

    return Unit.Value;
  }
}

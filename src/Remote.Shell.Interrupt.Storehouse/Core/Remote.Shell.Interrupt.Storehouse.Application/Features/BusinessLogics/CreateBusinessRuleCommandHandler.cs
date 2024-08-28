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
    Expression<Func<BusinessRule, bool>> filter = x => x.Name == request.CreateBusinessRuleDTO
                                                                        .Name;
    var existingBusinessRule = await _businessRuleRepository.ExistsAsync(filterExpression: filter,
                                                                         cancellationToken: cancellationToken);

    if (existingBusinessRule)
      throw new EntityAlreadyExists(request.CreateBusinessRuleDTO
                                           .Name);

    var businessRules = await _businessRuleRepository.GetAllAsync(cancellationToken);

    var addingRule = request.CreateBusinessRuleDTO
                            .Adapt<BusinessRule>();

    addingRule.Created = DateTime.Now;
    addingRule.Modified = DateTime.Now;

    if (businessRules.Count == 0)
    {
      addingRule.Branch = [1];
      addingRule.SequenceNumber = 0;
    }
    else if (businessRules.Count >= 1)
    {
      var requestBranches = request.CreateBusinessRuleDTO.Branch;
      var filteredBusinessRules = businessRules.Where(x => x.Branch != null &&
                                                      x.Branch.SequenceEqual(requestBranches))
                                               .ToList();

      var countOfElementsInBranch = filteredBusinessRules.Count;

      if (countOfElementsInBranch == 0)
      {
        var branchSubsequence = request.CreateBusinessRuleDTO
                                       .Branch
                                       .Take(request.CreateBusinessRuleDTO
                                                    .Branch
                                                    .Count - 1)
                                       .ToList();

        filteredBusinessRules = businessRules.Where(x => x.Branch
                                                          .SequenceEqual(branchSubsequence))
                                             .ToList();

        if (filteredBusinessRules.Count == 0)
        {
          throw new ArgumentException("The adding business rule has wrong brnach.");
        }
        else if (filteredBusinessRules.Count != 0)
        {
          var preCount = branchSubsequence.Count + 1;
          var branchPreCount = businessRules.Where(x => x.Branch.Count == preCount);

          if (branchPreCount.Count() == 0)
          {
            addingRule.Branch = branchSubsequence;
            addingRule.Branch.Add(1);
          }
          else
          {
            addingRule.Branch = [];
          }
          addingRule.SequenceNumber = 0;
        }
      }
      else if (countOfElementsInBranch != 0)
      {
        var maxSequenceNumber = filteredBusinessRules.Max(rule => rule.SequenceNumber);

        addingRule.SequenceNumber = ++maxSequenceNumber;
      }
    }
    else
      throw new Exception($"A business rules with \"{request.CreateBusinessRuleDTO
                                                            .Branch.ToString()}\" subsuequence doesn't exists.");

    await _businessRuleRepository.InsertOneAsync(document: addingRule,
                                                 cancellationToken: cancellationToken);

    return Unit.Value;
  }
}

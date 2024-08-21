namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics;

public record CreateBusinessRuleCommand(string Name,
                                        Expression<Func<object, bool>>? Condition,
                                        int[] Branch,
                                        Assignment? Assignment) : ICommand;

internal class CreateBusinessRuleCommandHandler(IBusinessRuleRepository businessRuleRepository)
  : ICommandHandler<CreateBusinessRuleCommand, Unit>
{
  readonly IBusinessRuleRepository _businessRuleRepository = businessRuleRepository
    ?? throw new ArgumentNullException(nameof(businessRuleRepository));

  async Task<Unit> IRequestHandler<CreateBusinessRuleCommand, Unit>.Handle(CreateBusinessRuleCommand request,
                                                                           CancellationToken cancellationToken)
  {
    // Check if adding business rule is exists by name
    var existingBusinessRule = await _businessRuleRepository.ExistsAsync(filterExpression: x => x.Name == request.Name,
                                                                         cancellationToken: cancellationToken);

    // If adding business rule is exists
    // throw EntityAlreadyExists exception
    if (existingBusinessRule)
      throw new EntityAlreadyExists(request.Name);

    // Get all business rules
    var businessRules = await _businessRuleRepository.GetAllAsync(cancellationToken);

    // convert input data to BusinessRule model
    var addingRule = request.Adapt<BusinessRule>();

    // Check if adding business rule is first
    if (businessRules.Count == 0)
    {
      // Set first branch number to 1
      addingRule.Branch = [1];
      // Set first sequence number to 0
      addingRule.SequenceNumber = 0;

      businessRules.Add(addingRule);
    }
    // If adding business rule is second or more
    else if (businessRules.Count > 1)
    {
      // Filtering the subsequence for the presence of a request branch number
      // For example request branch is [1, 3, 15] and looking for [1, 3, 15]
      var filteredBusinessRules = businessRules.Where(x => x.Branch == request.Branch).ToList();
      // Count of elements in subsequence
      var countOfElementsInBranch = filteredBusinessRules.Count;

      // Count of elements in previous subsequence greater than 0
      // For example countOfElementsInBranch is 5
      if (countOfElementsInBranch != 0)
      {
        // Get max value of SequenceNumber
        // For example business rules has model with the SequenceNumber equal 14
        var maxSequenceNumber = filteredBusinessRules.Max(rule => rule.SequenceNumber);

        // Set of max value of SequenceNumber + 1
        // 14 + 1 = 15
        addingRule.SequenceNumber = maxSequenceNumber++;

        // Save new BusinessRule
        businessRules.Add(addingRule);
      }
      // Count of elements in previous subsequence equal 0
      // For example countOfElementsInBranch is 0
      else if (countOfElementsInBranch == 0)
      {
        // Extract branch subsequence without the last element
        // For example request branch is [1, 3, 15] and looking for [1, 3]
        var branchSubsequence = request.Branch.Take(request.Branch.Length - 1).ToArray();
        filteredBusinessRules = businessRules.Where(x => x.Branch.SequenceEqual(branchSubsequence)).ToList();

        // The count of elements of filteredBusinessRules equals 0
        // It means it's second branch
        if (filteredBusinessRules.Count == 0)
        {
          // Handle new branch scenario
          // You can set the sequence number or perform other actions here
          // For example:
          addingRule.Branch = request.Branch;
          addingRule.SequenceNumber = 0; // Default sequence number for the new branch
          businessRules.Add(addingRule);
        }
      }
    }
    else
      // TODO new exception for that type of exceptions
      throw new Exception($"A business rules with \"{request.Branch.ToString()}\" subsuequence doesn't exists.");

    await _businessRuleRepository.InsertOneAsync(document: addingRule,
                                                 cancellationToken: cancellationToken);

    return Unit.Value;
  }
}

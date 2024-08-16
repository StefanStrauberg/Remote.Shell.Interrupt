namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics;

public record CreateBusinessRuleCommand(string Name,
                                        Expression<Func<object, bool>>? Condition,
                                        int BranchNumber,
                                        int SequenceNumber,
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
    var existingBusinessRule = await _businessRuleRepository.ExistsAsync(x => x.Name == request.Name,
                                                                         cancellationToken);

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
      addingRule.BranchNumber = 1;
      // Set first sequence number to 0
      addingRule.SequenceNumber = 0;

      businessRules.Add(addingRule);
    }
    // If adding business rule is second or more
    else if (businessRules.Count > 1)
    {
      // Filtering the subsequence for the presence of a request branch number
      var filteredBusinessRules = businessRules.Where(x => x.BranchNumber == request.BranchNumber).ToList();

      // Count of elements in previous subsequence greater than 0
      if (filteredBusinessRules.Count != 0)
      {
        // Get max value of SequenceNumber
        var maxSequenceNumber = filteredBusinessRules.Max(rule => rule.SequenceNumber);

        // Set of max value of SequenceNumber + 1
        addingRule.SequenceNumber = maxSequenceNumber++;

        // Save new BusinessRule
        businessRules.Add(addingRule);
      }
      // Count of elements in previous subsequence equal 0
      else if (filteredBusinessRules.Count == 0)
      {
        // Filtering the sequence for the presence of a request subsequence without last element from the request subsequence
        filteredBusinessRules = businessRules.Where(x => x.BranchNumber == request.BranchNumber / 10).ToList();

        // The count of elements of filteredBusinessRules equals 0
        // It means it's second branch
        if (filteredBusinessRules.Count == 0)
        {

        }
        // Otherwise we looking for that number
        else
        {

        }
      }
      else
        // TODO new exception for that type of exceptions
        throw new Exception($"A business rules with \"{request.BranchNumber.ToString()}\" subsuequence doesn't exists.");
    }

    return Unit.Value;
  }
}

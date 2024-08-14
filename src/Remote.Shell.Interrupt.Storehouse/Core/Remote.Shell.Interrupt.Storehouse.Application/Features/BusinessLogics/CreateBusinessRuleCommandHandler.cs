
namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics;

public record CreateBusinessRuleCommand(string RuleName,
                                        bool Last,
                                        Guid NextHopIfTrue,
                                        Guid NextHopIfFalse,
                                        Expression<Func<object, bool>>? Condition) : ICommand;

public class CreateBusinessRuleCommandHandler(IBusinessRuleRepository businessRuleRepository)
  : ICommandHandler<CreateBusinessRuleCommand, Unit>
{
  readonly IBusinessRuleRepository _businessRuleRepository = businessRuleRepository
    ?? throw new ArgumentNullException(nameof(businessRuleRepository));

  public async Task<Unit> IRequestHandler<CreateBusinessRuleCommand, Unit>.Handle(CreateBusinessRuleCommand request,
                                                                                  CancellationToken cancellationToken)
  {
    var existingBusinessRule = await _businessRuleRepository.ExistsAsync(x => x.RuleName == request.RuleName,
                                                                         cancellationToken);

    if (existingBusinessRule)
      throw new EntityAlreadyExists(request.RuleName);

    Dictionary<int, BusinessRule> rules;

    var rules = await _businessRuleRepository.GetAllAsync(cancellationToken);

    var businessRule = new BusinessRule()
    {
      RuleName = request.RuleName,

    };
  }
}

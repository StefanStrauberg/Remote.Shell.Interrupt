namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics;

public record CreateBusinessRuleCommand(string RuleName,
                                        bool Last,
                                        Guid NextHopIfTrue,
                                        Guid NextHopIfFalse,
                                        Expression<Func<object, bool>>? Condition) : ICommand;

internal class CreateBusinessRuleCommandHandler(IBusinessRuleRepository businessRuleRepository)
  : ICommandHandler<CreateBusinessRuleCommand, Unit>
{
  readonly IBusinessRuleRepository _businessRuleRepository = businessRuleRepository
    ?? throw new ArgumentNullException(nameof(businessRuleRepository));

  async Task<Unit> IRequestHandler<CreateBusinessRuleCommand, Unit>.Handle(CreateBusinessRuleCommand request,
                                                                           CancellationToken cancellationToken)
  {
    var existingBusinessRule = await _businessRuleRepository.ExistsAsync(x => x.RuleName == request.RuleName,
                                                                         cancellationToken);

    if (existingBusinessRule)
      throw new EntityAlreadyExists(request.RuleName);

    var businessRules = await _businessRuleRepository.GetAllAsync(cancellationToken);
    var rules = businessRules.ToDictionary(x => x.Id);

    if (rules != null)
    {

    }

    return Unit.Value;
  }
}

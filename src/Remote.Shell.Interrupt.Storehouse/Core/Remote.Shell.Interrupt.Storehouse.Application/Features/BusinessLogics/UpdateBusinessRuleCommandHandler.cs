namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics;

public class UpdateBusinessRule
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public Expression<Func<object, bool>>? Condition { get; set; }
  public int[] Branch { get; set; } = [];
  public int SequenceNumber { get; set; }
  public Assignment? Assignment { get; set; }
};

public class UpdateBusinessRuleDTOValidator : AbstractValidator<UpdateBusinessRule>
{
  public UpdateBusinessRuleDTOValidator()
  {
    RuleFor(x => x.Name).NotNull().WithMessage("Name can't be null")
                        .NotEmpty().WithMessage("Name can't be empty");

    RuleFor(x => x.Branch).NotNull().WithMessage("Name can't be null");

    RuleFor(x => x.Branch).NotNull().WithMessage("Name can't be null");
  }
}

public record UpdateBusinessRuleCommand(Guid Id,
                                        UpdateBusinessRule UpdateBusinessRule)
  : ICommand;


internal class UpdateBusinessRuleCommandHandler(IBusinessRuleRepository businessRuleRepository)
  : ICommandHandler<UpdateBusinessRuleCommand, Unit>
{
  readonly IBusinessRuleRepository _businessRuleRepository = businessRuleRepository
    ?? throw new ArgumentNullException(nameof(businessRuleRepository));

  async Task<Unit> IRequestHandler<UpdateBusinessRuleCommand, Unit>.Handle(UpdateBusinessRuleCommand request,
                                                                           CancellationToken cancellationToken)
  {
    // Looking for by ID, Branch and SequenceNumber
    Expression<Func<BusinessRule, bool>> filter = x => x.Id == request.UpdateBusinessRule.Id &&
                                                       x.Branch == request.UpdateBusinessRule.Branch &&
                                                       x.SequenceNumber == request.UpdateBusinessRule.SequenceNumber;

    var existingUpdatingBusinessRule = await _businessRuleRepository.ExistsAsync(filterExpression: filter,
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

namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics;


public class UpdateBusinessRuleDTOValidator : AbstractValidator<UpdateBusinessRuleDTO>
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
    // Looking for by ID, Branch and SequenceNumber
    Expression<Func<BusinessRule, bool>> filter = x => x.Id == request.Id &&
                                                       x.Branch == request.UpdateBusinessRule.Branch &&
                                                       x.SequenceNumber == request.UpdateBusinessRule.SequenceNumber;

    var existingUpdatingBusinessRule = await _businessRuleRepository.ExistsAsync(filterExpression: filter,
                                                                                 cancellationToken: cancellationToken);

    if (!existingUpdatingBusinessRule)
      throw new EntityNotFoundException(request.Id.ToString());

    var updatingBusinessRule = request.Adapt<BusinessRule>();

    updatingBusinessRule.Id = request.Id;
    updatingBusinessRule.Modified = DateTime.Now;

    await _businessRuleRepository.ReplaceOneAsync(filterExpression: x => x.Id == request.Id,
                                                  document: updatingBusinessRule,
                                                  cancellationToken: cancellationToken);
    return Unit.Value;
  }
}

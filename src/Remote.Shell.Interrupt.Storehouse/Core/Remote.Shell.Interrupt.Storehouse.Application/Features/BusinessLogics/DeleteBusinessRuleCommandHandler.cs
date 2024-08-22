namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics;

public record DeleteBusinessRuleByExpressionCommand(Expression<Func<BusinessRule, bool>> FilterExpression)
  : ICommand
{

}

internal class DeleteBusinessRuleByExpressionCommandHandler(IBusinessRuleRepository businessRuleRepository)
  : ICommandHandler<DeleteBusinessRuleByExpressionCommand, Unit>
{
  readonly IBusinessRuleRepository _businessRuleRepository = businessRuleRepository
    ?? throw new ArgumentNullException(nameof(businessRuleRepository));

  async Task<Unit> IRequestHandler<DeleteBusinessRuleByExpressionCommand, Unit>.Handle(DeleteBusinessRuleByExpressionCommand request,
                                                                                       CancellationToken cancellationToken)
  {
    var existingBusinessRule = await _businessRuleRepository.ExistsAsync(filterExpression: request.FilterExpression,
                                                                         cancellationToken: cancellationToken);

    if (!existingBusinessRule)
      throw new EntityNotFoundException(request.ToString());

    await _businessRuleRepository.DeleteOneAsync(filterExpression: request.FilterExpression,
                                                 cancellationToken: cancellationToken);
    return Unit.Value;
  }
}

namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics.Commands.Update;

public record UpdateBusinessRuleCommand(UpdateBusinessRuleDTO UpdateBusinessRule)
  : ICommand;

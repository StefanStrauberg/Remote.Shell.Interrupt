namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Assignments.Commands.Delete;

public record DeleteAssignmentByExpressionCommand(Expression<Func<Assignment, bool>> FilterExpression)
  : ICommand;

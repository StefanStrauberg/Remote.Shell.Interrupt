namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Assignments;

public record DeleteAssignmentByExpressionCommand(Expression<Func<Assignment, bool>> FilterExpression)
  : ICommand;

internal class DeleteAssignmentByExpressionCommandHandler(IAssignmentRepository assignmentRepository)
  : ICommandHandler<DeleteAssignmentByExpressionCommand, Unit>
{
  readonly IAssignmentRepository _assignmentRepository = assignmentRepository
    ?? throw new ArgumentNullException(nameof(assignmentRepository));

  async Task<Unit> IRequestHandler<DeleteAssignmentByExpressionCommand, Unit>.Handle(DeleteAssignmentByExpressionCommand request,
                                                                                     CancellationToken cancellationToken)
  {
    // Проверка существует ли назначение, соответствующее выражению фильтра
    var existingAssignment = await _assignmentRepository.ExistsAsync(filterExpression: request.FilterExpression,
                                                                     cancellationToken: cancellationToken);

    // Если назначение не найдено, выбрасываем исключение
    if (!existingAssignment)
      throw new EntityNotFoundException(new ExpressionToStringConverter<Assignment>().Convert(request.FilterExpression));

    // Удаление назначения из репозитория
    await _assignmentRepository.DeleteOneAsync(filterExpression: request.FilterExpression,
                                               cancellationToken: cancellationToken);

    // Возврат успешного завершения операции
    return Unit.Value;
  }
}

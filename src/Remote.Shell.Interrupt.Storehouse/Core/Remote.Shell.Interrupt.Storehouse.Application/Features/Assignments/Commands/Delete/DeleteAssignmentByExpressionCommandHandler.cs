namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Assignments.Commands.Delete;

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

    // Находим назначение, которое нужно удалить, по выражению фильтра
    var assignmentToDelete = await _assignmentRepository.FindOneAsync(filterExpression: request.FilterExpression,
                                                                      cancellationToken: cancellationToken);

    // Удаление назначения из репозитория
    await _assignmentRepository.DeleteOneAsync(document: assignmentToDelete,
                                               cancellationToken: cancellationToken);

    // Возврат успешного завершения операции
    return Unit.Value;
  }
}

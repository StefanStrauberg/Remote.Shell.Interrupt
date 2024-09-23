namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Assignments.Commands.Delete;

internal class DeleteAssignmentByExpressionCommandHandler(IUnitOfWork unitOfWork)
  : ICommandHandler<DeleteAssignmentByExpressionCommand, Unit>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));

  async Task<Unit> IRequestHandler<DeleteAssignmentByExpressionCommand, Unit>.Handle(DeleteAssignmentByExpressionCommand request,
                                                                                     CancellationToken cancellationToken)
  {
    // Проверка существует ли назначение, соответствующее выражению фильтра
    var existingAssignment = await _unitOfWork.Assignments
                                              .AnyAsync(predicate: request.FilterExpression,
                                                        cancellationToken: cancellationToken);

    // Если назначение не найдено, выбрасываем исключение
    if (!existingAssignment)
      throw new EntityNotFoundException(new ExpressionToStringConverter<Assignment>().Convert(request.FilterExpression));

    // Находим назначение, которое нужно удалить, по выражению фильтра
    var assignmentToDelete = await _unitOfWork.Assignments
                                              .FirstAsync(predicate: request.FilterExpression,
                                                          cancellationToken: cancellationToken);

    // Удаление назначения из репозитория
    _unitOfWork.Assignments.DeleteOne(entity: assignmentToDelete);
    await _unitOfWork.CompleteAsync(cancellationToken);

    // Возврат успешного завершения операции
    return Unit.Value;
  }
}

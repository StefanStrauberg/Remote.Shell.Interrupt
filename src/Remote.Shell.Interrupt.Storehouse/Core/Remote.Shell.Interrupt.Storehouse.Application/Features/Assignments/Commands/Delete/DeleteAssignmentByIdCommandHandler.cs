namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Assignments.Commands.Delete;

public record DeleteAssignmentByIdCommand(Guid Id)
  : ICommand;

internal class DeleteAssignmentByIdCommandHandler(IUnitOfWork unitOfWork)
  : ICommandHandler<DeleteAssignmentByIdCommand, Unit>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));

  async Task<Unit> IRequestHandler<DeleteAssignmentByIdCommand, Unit>.Handle(DeleteAssignmentByIdCommand request,
                                                                             CancellationToken cancellationToken)
  {
    // Проверка существования назначения с данным ID
    var existingAssignment = await _unitOfWork.Assignments
                                              .AnyByIdAsync(request.Id,
                                                            cancellationToken);

    // Если назначение не найдено — исключение
    if (!existingAssignment)
      throw new EntityNotFoundById(typeof(Assignment),
                                   request.Id.ToString());

    // Получение назначения для удаления
    var assignmentToDelete = await _unitOfWork.Assignments
                                              .FirstByIdAsync(request.Id,
                                                              cancellationToken);

    // Удаление назначения
    _unitOfWork.Assignments.DeleteOne(entity: assignmentToDelete);
    _unitOfWork.Complete();

    // Возврат успешного завершения операции
    return Unit.Value;
  }
}

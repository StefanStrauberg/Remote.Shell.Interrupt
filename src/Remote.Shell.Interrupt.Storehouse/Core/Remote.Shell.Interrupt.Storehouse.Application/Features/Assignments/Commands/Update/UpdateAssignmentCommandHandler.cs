namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Assignments.Commands.Update;

public record UpdateAssignmentCommand(UpdateAssignmentDTO UpdateAssignmentDTO) : ICommand;

internal class UpdateAssignmentCommandHandler(IUnitOfWork unitOfWork,
                                              IMapper mapper)
  : ICommandHandler<UpdateAssignmentCommand, Unit>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<Unit> IRequestHandler<UpdateAssignmentCommand, Unit>.Handle(UpdateAssignmentCommand request,
                                                                         CancellationToken cancellationToken)
  {
    // Проверка существования назначения с указанным ID
    var existingUpdatingAssignmentById = await _unitOfWork.Assignments
                                                          .AnyByIdAsync(request.UpdateAssignmentDTO.Id,
                                                                        cancellationToken);

    // Если назначение не найдено — исключение
    if (!existingUpdatingAssignmentById)
      throw new EntityNotFoundById(typeof(Assignment),
                                   request.UpdateAssignmentDTO.Id.ToString());

    // Проверка, существует ли назначение с таким же именем и другим ID
    var existingUpdatingAssignmentByName = await _unitOfWork.Assignments
                                                            .AnyWithTheSameNameAndDifferentIdAsync(request.UpdateAssignmentDTO.Id,
                                                                                                   request.UpdateAssignmentDTO.Name,
                                                                                                   cancellationToken);

    // Если назначение с таким именем и другим ID существует — исключение
    if (existingUpdatingAssignmentByName)
      throw new EntityAlreadyExists($"the name \"{request.UpdateAssignmentDTO.Name}\" and with the Id \"{request.UpdateAssignmentDTO.Id}\"");

    // Получаем назначение для обновления
    var assignment = await _unitOfWork.Assignments
                                      .FirstByIdAsync(request.UpdateAssignmentDTO.Id,
                                                      cancellationToken);

    // Преобразуем DTO в доменную модель назначения
    _mapper.Map(request.UpdateAssignmentDTO, assignment);

    // Обновляем назначение в базе данных
    _unitOfWork.Assignments.ReplaceOne(assignment);

    // Подтверждаем изменения
    await _unitOfWork.CompleteAsync(cancellationToken);

    // Возврат успешного завершения операции
    return Unit.Value;
  }
}

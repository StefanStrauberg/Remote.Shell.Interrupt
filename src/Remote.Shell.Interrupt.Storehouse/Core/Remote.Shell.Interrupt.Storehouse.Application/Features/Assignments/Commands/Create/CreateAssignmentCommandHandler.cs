namespace Remote.Shell.Interrupt.Storehouse.Application.Features.Assignments.Commands.Create;

public record CreateAssignmentCommand(CreateAssignmentDTO CreateAssignmentDTO) : ICommand;

internal class CreateAssignmentCommandHandler(IUnitOfWork unitOfWork,
                                              IMapper mapper)
  : ICommandHandler<CreateAssignmentCommand, Unit>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<Unit> IRequestHandler<CreateAssignmentCommand, Unit>.Handle(CreateAssignmentCommand request,
                                                                         CancellationToken cancellationToken)
  {
    // Проверка существования назначения с таким именем
    var existingAssignment = await _unitOfWork.Assignments
                                              .AnyWithTheSameNameAsync(request.CreateAssignmentDTO.Name,
                                                                       cancellationToken);

    // Если назначение с таким именем существует — исключение
    if (existingAssignment)
      throw new EntityAlreadyExists($"the name \"{request.CreateAssignmentDTO.Name}\"");

    // Преобразование DTO в доменную модель
    var assignment = _mapper.Map<Assignment>(request.CreateAssignmentDTO);

    // Сохранение назначения в базе данных
    _unitOfWork.Assignments
               .InsertOne(assignment);

    // Подтверждение изменений
    _unitOfWork.Complete();

    // Возврат успешного завершения операции
    return Unit.Value;
  }
}

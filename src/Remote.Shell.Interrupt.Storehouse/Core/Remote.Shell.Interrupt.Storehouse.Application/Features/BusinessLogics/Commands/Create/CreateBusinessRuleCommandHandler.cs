namespace Remote.Shell.Interrupt.Storehouse.Application.Features.BusinessLogics.Commands.Create;

public record CreateBusinessRuleCommand(CreateBusinessRuleDTO CreateBusinessRuleDTO) : ICommand;

internal class CreateBusinessRuleCommandHandler(IUnitOfWork unitOfWork,
                                                IMapper mapper)
  : ICommandHandler<CreateBusinessRuleCommand, Unit>
{
  readonly IUnitOfWork _unitOfWork = unitOfWork
    ?? throw new ArgumentNullException(nameof(unitOfWork));
  readonly IMapper _mapper = mapper
    ?? throw new ArgumentNullException(nameof(mapper));

  async Task<Unit> IRequestHandler<CreateBusinessRuleCommand, Unit>.Handle(CreateBusinessRuleCommand request,
                                                                           CancellationToken cancellationToken)
  {
    // Проверка, указан ли ID назначения для бизнес-правила
    if (request.CreateBusinessRuleDTO.AssignmentId != null)
    {
      // Проверка существует ли назначение с указанным ID
      var existingAssignment = await _unitOfWork.Assignments
                                                .AnyByIdAsync(request.CreateBusinessRuleDTO.AssignmentId.Value, cancellationToken);

      // Если назначение не найдено, выбрасываем исключение
      if (!existingAssignment)
        throw new EntityNotFoundById(typeof(Assignment), request.CreateBusinessRuleDTO.AssignmentId.ToString()!);
    }

    // Преобразование DTO в сущность бизнес-правила
    var addingBusinessRule = _mapper.Map<BusinessRule>(request.CreateBusinessRuleDTO);

    // Проверка существуют ли какие либо бизнес-правила
    var isAny = await _unitOfWork.BusinessRules
                                 .AnyAsync(cancellationToken);

    if (!isAny)
    {
      addingBusinessRule.IsRoot = true;
      addingBusinessRule.Parent = null;
    }
    else
      addingBusinessRule.IsRoot = false;

    // Проверка наличия родительского бизнес-правила
    if (request.CreateBusinessRuleDTO.ParentId != null && isAny)
    {
      // Проверка существует ли бизнес-правило с указанным ID
      var existingBusinessRuleById = await _unitOfWork.BusinessRules.AnyByIdAsync(request.CreateBusinessRuleDTO.ParentId.Value,
                                                                                  cancellationToken);

      // Если назначение с таким ID не найдено, выбрасываем исключение
      if (!existingBusinessRuleById)
        throw new EntityNotFoundById(typeof(BusinessRule),
                                     request.CreateBusinessRuleDTO.ParentId.Value.ToString());

      // Получаем бизнес правило 
      var parentBusinessRule = await _unitOfWork.BusinessRules
                                                .FirstByIdAsync(request.CreateBusinessRuleDTO.ParentId.Value,
                                                                cancellationToken);

      // Вставка нового бизнес-правила в репозиторий
      _unitOfWork.BusinessRules
                 .InsertOne(addingBusinessRule);

      // Добавление ID нового бизнес-правила в список дочерних элементов родительского бизнес-правила
      parentBusinessRule.Children
                        .Add(addingBusinessRule);

      // Обновление родительского бизнес-правила в репозитории
      _unitOfWork.BusinessRules
                 .ReplaceOne(parentBusinessRule);
    }
    else
    {
      // Если нет родительского бизнес-правила, просто вставляем новое бизнес-правило в репозиторий
      _unitOfWork.BusinessRules
                 .InsertOne(entity: addingBusinessRule);
    }

    await _unitOfWork.CompleteAsync(cancellationToken);

    // Возврат успешного завершения операции
    return Unit.Value;
  }
}

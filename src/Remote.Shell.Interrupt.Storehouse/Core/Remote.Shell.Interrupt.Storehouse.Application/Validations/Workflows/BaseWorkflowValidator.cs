namespace Remote.Shell.Interrupt.Storehouse.Application.Validations.Workflows;

public class BaseWorkflowValidator<T, TDto> : AbstractValidator<T> where TDto : BaseWorkflowDTO
{
  protected Func<T, TDto> _selector;

  public BaseWorkflowValidator(Func<T, TDto> selector)
  {
    _selector = selector;

    RuleFor(x => selector(x).Name).NotNull()
                                  .WithMessage("{PropertyName} can't be null")
                                  .NotEmpty()
                                  .WithMessage("{PropertyName} is required");

    RuleFor(x => selector(x).Nodes).NotEmpty()
                                   .WithMessage("A workflow must contain at least one node");

    RuleFor(x => selector(x)).Must(HaveKnownNodeTypes)
                             .WithMessage($"Every node's Type must be one of: {string.Join(", ", WorkflowNodeTypes.All)}")
                             .Must(HaveExactlyOneStartNode)
                             .WithMessage("A workflow must contain exactly one Start node")
                             .Must(HaveAtLeastOneEndNode)
                             .WithMessage("A workflow must contain at least one End node")
                             .Must(HaveValidStartNodeId)
                             .WithMessage("StartNodeId must match one of the workflow's nodes")
                             .Must(HaveEdgesReferencingKnownNodes)
                             .WithMessage("Every edge's FromNodeId/ToNodeId must match one of the workflow's nodes")
                             .Must(HaveNonEmptyScriptSource)
                             .WithMessage("Every Script node requires a non-empty config.scriptSource")
                             .Must(HaveValidScriptTimeout)
                             .WithMessage("Every Script node's config.timeoutMs, if present, must be a number between 50 and 30000")
                             .When(x => selector(x).Nodes.Count > 0);
  }

  static bool HaveKnownNodeTypes(TDto dto)
    => dto.Nodes.All(n => WorkflowNodeTypes.All.Contains(n.Type));

  static bool HaveExactlyOneStartNode(TDto dto)
    => dto.Nodes.Count(n => string.Equals(n.Type, WorkflowNodeTypes.Start, StringComparison.OrdinalIgnoreCase)) == 1;

  static bool HaveAtLeastOneEndNode(TDto dto)
    => dto.Nodes.Any(n => string.Equals(n.Type, WorkflowNodeTypes.End, StringComparison.OrdinalIgnoreCase));

  static bool HaveValidStartNodeId(TDto dto)
    => dto.Nodes.Any(n => n.Id == dto.StartNodeId);

  static bool HaveEdgesReferencingKnownNodes(TDto dto)
  {
    var nodeIds = dto.Nodes.Select(n => n.Id).ToHashSet();
    return dto.Edges.All(e => nodeIds.Contains(e.FromNodeId) && nodeIds.Contains(e.ToNodeId));
  }

  static IEnumerable<WorkflowNodeDTO> ScriptNodes(TDto dto)
    => dto.Nodes.Where(n => string.Equals(n.Type, WorkflowNodeTypes.Script, StringComparison.OrdinalIgnoreCase));

  static bool HaveNonEmptyScriptSource(TDto dto)
    => ScriptNodes(dto).All(n => n.Config.GetValueOrDefault("scriptSource") is string source && !string.IsNullOrWhiteSpace(source));

  static bool HaveValidScriptTimeout(TDto dto)
    => ScriptNodes(dto).All(n =>
    {
      var raw = n.Config.GetValueOrDefault("timeoutMs");
      if (raw is null)
        return true;

      try
      {
        var timeout = Convert.ToDouble(raw);
        return timeout is >= 50 and <= 30000;
      }
      catch (Exception ex) when (ex is InvalidCastException or FormatException or OverflowException)
      {
        return false;
      }
    });
}

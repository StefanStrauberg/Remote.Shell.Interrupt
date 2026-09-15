using Remote.Shell.Interrupt.Storehouse.Infrastructure.WorkflowEngine.NodeExecutors;

namespace Remote.Shell.Interrupt.Storehouse.Infrastructure.WorkflowEngine;

/// <summary>
/// Provides extension methods for registering workflow engine services.
/// </summary>
public static class WorkflowEngineServicesRegistration
{
  /// <summary>
  /// Registers the <see cref="IWorkflowEngine"/> implementation, its node resolver, and
  /// every built-in <see cref="IWorkflowNode"/> executor.
  /// </summary>
  public static IServiceCollection AddWorkflowEngineServices(this IServiceCollection services)
  {
    services.AddTransient<IWorkflowEngine, WorkflowEngine>();
    services.AddTransient<IWorkflowNodeResolver, WorkflowNodeResolver>();

    services.AddTransient<IWorkflowNode, StartNodeExecutor>();
    services.AddTransient<IWorkflowNode, EndNodeExecutor>();
    services.AddTransient<IWorkflowNode, DecisionNodeExecutor>();
    services.AddTransient<IWorkflowNode, JoinNodeExecutor>();
    services.AddTransient<IWorkflowNode, SetVariableNodeExecutor>();
    services.AddTransient<IWorkflowNode, SnmpGetNodeExecutor>();
    services.AddTransient<IWorkflowNode, SnmpWalkNodeExecutor>();
    services.AddTransient<IWorkflowNode, ScriptNodeExecutor>();

    return services;
  }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Logger;
using Remote.Shell.Interrupt.Storehouse.Application.Contracts.Repositories.UnOfWrkRep;
using Remote.Shell.Interrupt.Storehouse.Application.Features.Workflows.Seed;
using Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Configuration;

namespace Remote.Shell.Interrupt.Storehouse.Dapper.Persistence.Workflow;

/// <summary>
/// Idempotent startup seeding for built-in workflows, mirroring <c>IdentitySeeder</c>: the
/// default "Network device discovery" workflow is inserted once, under its fixed
/// <see cref="DefaultNetworkDeviceWorkflowFactory.WorkflowId"/>, and left alone on every
/// later startup once it exists - it's runtime-seeded data, not a schema change, so no EF
/// migration is involved.
/// </summary>
public sealed class WorkflowSeeder
{
  public static async Task SeedDefaultWorkflowsAsync(
    IServiceProvider serviceProvider,
    CancellationToken cancellationToken = default)
  {
    using var scope = serviceProvider.CreateScope();
    var provider = scope.ServiceProvider;

    var dbContext = provider.GetRequiredService<ApplicationDbContext>();
    var workflowUnitOfWork = provider.GetRequiredService<IWorkflowUnitOfWork>();
    var logger = provider.GetRequiredService<IAppLogger<WorkflowSeeder>>();

    var workflowId = DefaultNetworkDeviceWorkflowFactory.WorkflowId;

    if (await dbContext.WorkflowDefinitions.AnyAsync(w => w.Id == workflowId, cancellationToken))
      return;

    var workflow = DefaultNetworkDeviceWorkflowFactory.Create();

    workflowUnitOfWork.Workflows.InsertOne(workflow);
    workflowUnitOfWork.Complete();

    logger.LogInformation("Seeded default workflow '{Name}' ({WorkflowId}).", workflow.Name, workflow.Id);
  }
}

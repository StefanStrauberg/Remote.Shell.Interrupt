using System.Net;
using System.Net.Http.Json;
using Remote.Shell.Interrupt.Storehouse.Application.DTOs.Workflows;
using Tests.Integration.Fixtures;
using Tests.Integration.Helpers;

namespace Tests.Integration;

[Collection("Integration")]
public class WorkflowEndToEndTests(ApiFactory factory)
{
  [Fact]
  public async Task DesignerGraph_CreateUpdateExecutePublishArchiveDelete_RoundTrips()
  {
    using var client = factory.CreateClient();
    client.UseBearerToken(await client.LoginAsAdminAsync());
    var start = Guid.NewGuid();
    var script = Guid.NewGuid();
    var end = Guid.NewGuid();
    var name = $"designer-{Guid.NewGuid():N}";
    var dto = new CreateWorkflowDTO
    {
      Name = name, Version = 1, StartNodeId = start,
      Nodes = [
        new() { Id = start, Name = "Start", Type = "Start", Key = "start" },
        new() { Id = script, Name = "Transform", Type = "Script", Key = "transform", PositionX = 280,
          Config = new() { ["input"] = "payload", ["output"] = "answer", ["timeoutMs"] = 1500,
            ["scriptSource"] = "function execute(input, context) { console.log('converted'); return input.value + 1; }" } },
        new() { Id = end, Name = "End", Type = "End", Key = "end", PositionX = 560 }
      ],
      Edges = [
        new() { Id = Guid.NewGuid(), FromNodeId = start, ToNodeId = script, Priority = 1 },
        new() { Id = Guid.NewGuid(), FromNodeId = script, ToNodeId = end, Priority = 1 }
      ]
    };
    var create = await client.PostAsJsonAsync("/api/v1/Workflows/CreateWorkflow", dto);
    create.StatusCode.Should().Be(HttpStatusCode.OK, await create.Content.ReadAsStringAsync());
    // CreateWorkflow returns the new workflow's ID directly - the caller (this test, and the
    // SPA's workflowsApi.create) doesn't need to re-derive it via a name-filtered list lookup,
    // which was ambiguous under a concurrent create/rename with the same name.
    var savedId = await create.Content.ReadFromJsonAsync<Guid>();
    savedId.Should().NotBe(Guid.Empty);

    var graph = await client.GetFromJsonAsync<WorkflowDTO>($"/api/v1/Workflows/GetWorkflowById/{savedId}");
    graph!.Name.Should().Be(name);
    graph.Status.Should().Be("Draft");
    graph.Nodes.Single(n => n.Id == script).PositionX.Should().Be(280);

    var update = new UpdateWorkflowDTO { Id = savedId, Name = name, Version = 2, StartNodeId = start, Nodes = graph.Nodes, Edges = graph.Edges };
    update.Nodes.Single(n => n.Id == script).PositionX = 312;
    var updated = await client.PutAsJsonAsync("/api/v1/Workflows/UpdateWorkflow", update);
    updated.StatusCode.Should().Be(HttpStatusCode.OK, await updated.Content.ReadAsStringAsync());
    var reloaded = await client.GetFromJsonAsync<WorkflowDTO>($"/api/v1/Workflows/GetWorkflowById/{savedId}");
    reloaded!.Version.Should().Be(2);
    reloaded.Nodes.Single(n => n.Id == script).PositionX.Should().Be(312);

    var run = await client.PostAsJsonAsync($"/api/v1/Workflows/ExecuteWorkflow/{savedId}", new ExecuteWorkflowRequestDTO
    {
      // Host is inert for this graph (Script-only, no SnmpGet/Walk node ever dials it) - just
      // needs to pass ExecuteWorkflowCommandValidator's SnmpTargetGuard, which now rejects
      // loopback the same way SNMPGet/SNMPWalk already do.
      Host = "192.168.101.8", Community = "test",
      Input = new() { ["payload"] = new Dictionary<string, object?> { ["value"] = 41 } }
    });
    run.StatusCode.Should().Be(HttpStatusCode.OK, await run.Content.ReadAsStringAsync());
    var result = await run.Content.ReadFromJsonAsync<WorkflowExecutionResultDTO>();
    result!.Success.Should().BeTrue(result.Error);
    result.Steps.Should().HaveCount(3);
    result.Steps[1].Logs.Should().Contain("converted");
    result.FinalVariables["answer"]!.ToString().Should().Be("42");

    (await client.PostAsync($"/api/v1/Workflows/PublishWorkflow/{savedId}", null)).StatusCode.Should().Be(HttpStatusCode.OK);
    (await client.PutAsJsonAsync("/api/v1/Workflows/UpdateWorkflow", update)).StatusCode.Should().Be(HttpStatusCode.BadRequest);
    (await client.PostAsync($"/api/v1/Workflows/ArchiveWorkflow/{savedId}", null)).StatusCode.Should().Be(HttpStatusCode.OK);
    (await client.DeleteAsync($"/api/v1/Workflows/DeleteWorkflowById/{savedId}")).StatusCode.Should().Be(HttpStatusCode.OK);
    (await client.GetAsync($"/api/v1/Workflows/GetWorkflowById/{savedId}")).StatusCode.Should().Be(HttpStatusCode.NotFound);
  }

  [Fact]
  public async Task CreateWorkflow_DuplicateName_SecondCallFailsAndFirstIdStillResolves()
  {
    // Regression for the race the old name-lookup approach was exposed to: two creates that
    // could momentarily share a name must never let a caller resolve the wrong ID. The unique
    // DB index on Name (see WorkflowDefinitionConfiguration) plus returning the ID directly from
    // CreateWorkflow's own response removes the ambiguity entirely - there's no lookup to race.
    using var client = factory.CreateClient();
    client.UseBearerToken(await client.LoginAsAdminAsync());
    var start = Guid.NewGuid();
    var end = Guid.NewGuid();
    var name = $"dup-{Guid.NewGuid():N}";
    var dto = new CreateWorkflowDTO
    {
      Name = name, Version = 1, StartNodeId = start,
      Nodes = [
        new() { Id = start, Name = "Start", Type = "Start", Key = "start" },
        new() { Id = end, Name = "End", Type = "End", Key = "end" }
      ],
      Edges = [new() { Id = Guid.NewGuid(), FromNodeId = start, ToNodeId = end, Priority = 1 }]
    };

    var firstCreate = await client.PostAsJsonAsync("/api/v1/Workflows/CreateWorkflow", dto);
    firstCreate.StatusCode.Should().Be(HttpStatusCode.OK, await firstCreate.Content.ReadAsStringAsync());
    var firstId = await firstCreate.Content.ReadFromJsonAsync<Guid>();

    var secondCreate = await client.PostAsJsonAsync("/api/v1/Workflows/CreateWorkflow", dto);
    secondCreate.StatusCode.Should().Be(HttpStatusCode.BadRequest);

    var graph = await client.GetFromJsonAsync<WorkflowDTO>($"/api/v1/Workflows/GetWorkflowById/{firstId}");
    graph!.Id.Should().Be(firstId);
    graph.Name.Should().Be(name);

    (await client.DeleteAsync($"/api/v1/Workflows/DeleteWorkflowById/{firstId}")).StatusCode.Should().Be(HttpStatusCode.OK);
  }
}

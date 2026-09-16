import { describe, expect, it, vi, beforeEach } from "vitest";
import { createWorkflow } from "../src/features/Workflows/domain/workflow/defaults";
import {
  duplicateAsDraft,
  importAsDraft,
  scriptSourcesDiffer,
  scriptSourcesOf,
  validateWorkflow,
  workflowPayload,
} from "../src/features/Workflows/domain/workflow/graph";
import { parseObject } from "../src/features/Workflows/domain/workflow/model";
import { workflowsApi } from "../src/features/Workflows/api/workflowsApi";
import httpClient from "../src/lib/api/httpClient";

describe("workflow graph compatibility", () => {
  it("creates a valid graph with GUID references", () => {
    const graph = createWorkflow();
    expect(validateWorkflow(graph)).toEqual([]);
    expect(graph.startNodeId).toMatch(/^[0-9a-f-]{36}$/);
    expect(graph.edges[0].fromNodeId).toBe(graph.startNodeId);
  });
  it("imports prototype IDs and retains nested config, layout and edge conditions", () => {
    const graph = createWorkflow();
    graph.nodes[0].id = "n1";
    graph.nodes[1].id = "n2";
    graph.startNodeId = "n1";
    graph.edges[0] = {
      id: "e1",
      fromNodeId: "n1",
      toNodeId: "n2",
      condition: "DEFAULT",
      priority: 0,
    };
    graph.status = "Published";
    graph.nodes[1].config = { nested: { enabled: true, rows: [1, null] } };
    const imported = importAsDraft(graph);
    expect(imported.status).toBe("Draft");
    expect(imported.startNodeId).toBe(imported.nodes[0].id);
    expect(imported.edges[0].fromNodeId).toBe(imported.nodes[0].id);
    expect(imported.edges[0].toNodeId).toBe(imported.nodes[1].id);
    expect(imported.edges[0]).toMatchObject({
      condition: "DEFAULT",
      priority: 0,
    });
    expect(imported.nodes[1].config).toEqual(graph.nodes[1].config);
    expect(imported.nodes[1].positionX).toBe(graph.nodes[1].positionX);
    expect(graph.nodes[0].id).toBe("n1");
  });
  it("uses new primary keys when cloning persisted graphs", () => {
    const graph = createWorkflow();
    const copy = importAsDraft(graph);
    expect(copy.id).not.toBe(graph.id);
    expect(copy.nodes[0].id).not.toBe(graph.nodes[0].id);
    expect(copy.edges[0].id).not.toBe(graph.edges[0].id);
  });
  it("accepts SaveNetworkDevice and nullable node keys", () => {
    const graph = createWorkflow();
    graph.nodes[1].type = "SaveNetworkDevice";
    graph.nodes[1].key = null;
    expect(importAsDraft(graph).nodes[1]).toMatchObject({
      type: "SaveNetworkDevice",
      key: null,
    });
  });
  it("rejects broken references and non-object configuration", () => {
    const graph = createWorkflow();
    graph.edges[0].toNodeId = "missing";
    expect(() => importAsDraft(graph)).toThrow("Invalid workflow edge");
    expect(() => parseObject("[]")).toThrow();
    expect(() => parseObject("null")).toThrow();
  });
  it("does not send client workflow ID or lifecycle status on create", () => {
    const payload = workflowPayload(createWorkflow());
    expect(payload).not.toHaveProperty("id");
    expect(payload).not.toHaveProperty("status");
  });
  it("validates script source and nonfinite timeouts", () => {
    const graph = createWorkflow();
    graph.nodes[1].type = "Script";
    graph.nodes[1].config = { scriptSource: "", timeoutMs: "invalid" };
    expect(
      validateWorkflow(graph).some((e) => e.includes("scriptSource"))
    ).toBe(true);
    expect(validateWorkflow(graph).some((e) => e.includes("timeout"))).toBe(
      true
    );
  });
});

describe("draft-copy version handling", () => {
  // Regression coverage for: "Create draft copy" used to bump the version on every copy,
  // including when duplicating a Draft that was never published - silently skipping version
  // numbers for a workflow that was never actually released as that prior version.
  it("keeps the same version when copying a Draft", () => {
    const graph = { ...createWorkflow(), status: "Draft" as const, version: 3 };
    const copy = duplicateAsDraft(graph);
    expect(copy.version).toBe(3);
    expect(copy.name).toBe(`${graph.name} copy`);
    expect(copy.status).toBe("Draft");
    expect(copy.id).not.toBe(graph.id);
  });
  it("bumps the version when copying a Published workflow", () => {
    const graph = {
      ...createWorkflow(),
      status: "Published" as const,
      version: 3,
    };
    const copy = duplicateAsDraft(graph);
    expect(copy.version).toBe(4);
  });
  it("bumps the version when copying an Archived workflow", () => {
    const graph = {
      ...createWorkflow(),
      status: "Archived" as const,
      version: 1,
    };
    const copy = duplicateAsDraft(graph);
    expect(copy.version).toBe(2);
  });
});

describe("Script node save-confirmation gate", () => {
  // Regression coverage for the "no client-side friction before shipping
  // server-executable code" bug: WorkflowEditorPage.save() only prompts for
  // confirmation when a save would actually change what a Script node runs.
  it("extracts only Script nodes' source, keyed by node id", () => {
    const graph = createWorkflow();
    graph.nodes[1].type = "Script";
    graph.nodes[1].config = { scriptSource: "return 1;" };
    expect(scriptSourcesOf(graph)).toEqual({
      [graph.nodes[1].id]: "return 1;",
    });
  });
  it("treats a graph with no Script nodes as an empty source map", () => {
    expect(scriptSourcesOf(createWorkflow())).toEqual({});
  });
  it("detects no difference for an identical snapshot", () => {
    const before = { "node-1": "return 1;" };
    const after = { "node-1": "return 1;" };
    expect(scriptSourcesDiffer(before, after)).toBe(false);
  });
  it("detects an edited script body", () => {
    const before = { "node-1": "return 1;" };
    const after = { "node-1": "return 2;" };
    expect(scriptSourcesDiffer(before, after)).toBe(true);
  });
  it("detects a newly added Script node", () => {
    const before = {};
    const after = { "node-1": "return 1;" };
    expect(scriptSourcesDiffer(before, after)).toBe(true);
  });
  it("detects a removed Script node", () => {
    const before = { "node-1": "return 1;" };
    const after = {};
    expect(scriptSourcesDiffer(before, after)).toBe(true);
  });
  it("ignores unrelated graph changes (e.g. renaming a node)", () => {
    const graph = createWorkflow();
    graph.nodes[1].type = "Script";
    graph.nodes[1].config = { scriptSource: "return 1;" };
    const before = scriptSourcesOf(graph);

    graph.nodes[1].name = "Renamed";
    graph.name = "Renamed workflow";
    const after = scriptSourcesOf(graph);

    expect(scriptSourcesDiffer(before, after)).toBe(false);
  });
});

describe("workflow HTTP contract", () => {
  beforeEach(() => vi.restoreAllMocks());
  it("uses the ID returned directly by CreateWorkflow, not a name lookup", async () => {
    // Regression: this used to re-derive the ID via list(1, name, true), which could resolve
    // to the wrong workflow under a concurrent create/rename sharing that name. The backend
    // now returns the new ID in the CreateWorkflow response body itself.
    const draft = createWorkflow();
    const saved = { ...draft, id: "server-id" };
    const post = vi
      .spyOn(httpClient, "post")
      .mockResolvedValue({ data: "server-id" });
    const list = vi.spyOn(workflowsApi, "list");
    vi.spyOn(workflowsApi, "get").mockResolvedValue(saved);

    expect(await workflowsApi.create(draft)).toEqual(saved);

    expect(post).toHaveBeenCalledWith(
      "/api/v1/Workflows/CreateWorkflow",
      workflowPayload(draft)
    );
    expect(workflowsApi.get).toHaveBeenCalledWith("server-id");
    expect(list).not.toHaveBeenCalled();
  });
  it("does not retry a successful creation if the follow-up GET fails", async () => {
    const post = vi
      .spyOn(httpClient, "post")
      .mockResolvedValue({ data: "server-id" });
    vi.spyOn(workflowsApi, "get").mockRejectedValue(new Error("Offline"));
    expect(await workflowsApi.create(createWorkflow())).toBeNull();
    expect(post).toHaveBeenCalledTimes(1);
  });
  it("preserves a workflow failure returned with HTTP 200 and sends cancellation", async () => {
    const result = {
      success: false,
      error: "No matching edge",
      steps: [],
      finalVariables: {},
    };
    const post = vi
      .spyOn(httpClient, "post")
      .mockResolvedValue({ data: result });
    const signal = new AbortController().signal;
    const request = {
      host: "127.0.0.1",
      community: "test",
      input: { "input.vendor": "Huawei" },
    };
    expect(await workflowsApi.execute("workflow-id", request, signal)).toEqual(
      result
    );
    expect(post).toHaveBeenCalledWith(
      "/api/v1/Workflows/ExecuteWorkflow/workflow-id",
      request,
      { signal, timeout: 0 }
    );
  });
});

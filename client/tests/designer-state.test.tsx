// @vitest-environment jsdom
import { act, renderHook } from "@testing-library/react";
import { PropsWithChildren } from "react";
import { describe, expect, it, vi } from "vitest";
import { DesignerProvider } from "../src/features/Workflows/designer/state/DesignerProvider";
import { useDesigner } from "../src/features/Workflows/designer/state/designerContext";
import {
  createWorkflow,
  GRID_SIZE,
} from "../src/features/Workflows/domain/workflow/defaults";
import { WorkflowDefinition } from "../src/features/Workflows/domain/workflow/model";

function designer(initial = createWorkflow()) {
  return renderHook(() => useDesigner(), {
    wrapper: ({ children }: PropsWithChildren) => (
      <DesignerProvider initial={initial}>{children}</DesignerProvider>
    ),
  });
}

describe("workflow designer state", () => {
  it("tracks unsaved edits, undo and redo without mutating the original graph", () => {
    const initial = createWorkflow();
    const { result } = designer(initial);
    expect(result.current.dirty).toBe(false);
    act(() => result.current.updateWorkflow({ name: "Renamed workflow" }));
    expect(result.current.dirty).toBe(true);
    expect(result.current.canUndo).toBe(true);
    expect(initial.name).not.toBe("Renamed workflow");
    act(() => result.current.undo());
    expect(result.current.workflow).toEqual(initial);
    expect(result.current.dirty).toBe(false);
    expect(result.current.canRedo).toBe(true);
    act(() => result.current.redo());
    expect(result.current.workflow.name).toBe("Renamed workflow");
  });
  it("discards the redo branch after a new edit", () => {
    const { result } = designer();
    act(() => result.current.updateWorkflow({ name: "First edit" }));
    act(() => result.current.undo());
    act(() => result.current.updateWorkflow({ name: "New branch" }));
    expect(result.current.canRedo).toBe(false);
    act(() => result.current.redo());
    expect(result.current.workflow.name).toBe("New branch");
  });
  it.each(["Published", "Archived"] as const)(
    "prevents graph changes for %s workflows",
    (status) => {
      const initial: WorkflowDefinition = { ...createWorkflow(), status };
      const { result } = designer(initial);
      act(() => {
        result.current.updateWorkflow({ name: "Should not change" });
        result.current.addNode("Script", 100, 100);
        result.current.moveNode(initial.startNodeId, 500, 500);
        result.current.deleteNode(initial.startNodeId);
        result.current.undo();
      });
      expect(result.current.readOnly).toBe(true);
      expect(result.current.workflow).toEqual(initial);
      expect(result.current.dirty).toBe(false);
      expect(result.current.canUndo).toBe(false);
    }
  );
  it("blocks edits while execution is busy and unlocks afterwards", () => {
    const { result } = designer();
    const original = result.current.workflow;
    act(() => result.current.setBusy(true));
    act(() => result.current.addNode("Script", 100, 100));
    expect(result.current.workflow).toEqual(original);
    act(() => result.current.setBusy(false));
    act(() => result.current.addNode("Script", 100, 100));
    expect(result.current.workflow.nodes).toHaveLength(
      original.nodes.length + 1
    );
  });
  it("keeps exactly one Start node when adding a replacement", () => {
    const { result } = designer();
    const oldStart = result.current.workflow.startNodeId;
    act(() => result.current.addNode("Start", 100, 100));
    const graph = result.current.workflow;
    expect(graph.nodes.filter((node) => node.type === "Start")).toHaveLength(1);
    expect(graph.nodes.find((node) => node.id === oldStart)?.type).toBe("Join");
    expect(graph.startNodeId).toBe(result.current.selectedNodeId);
  });
  it("duplicates Start as a Join with an independent identity", () => {
    const { result } = designer();
    const start = result.current.workflow.startNodeId;
    act(() => result.current.duplicateNode(start));
    expect(result.current.selectedNode?.id).not.toBe(start);
    expect(result.current.selectedNode?.type).toBe("Join");
    expect(result.current.workflow.startNodeId).toBe(start);
    expect(
      result.current.workflow.nodes.filter((node) => node.type === "Start")
    ).toHaveLength(1);
  });
  it("inserts a script into an edge and reconnects the original path on removal", () => {
    const { result } = designer();
    const edge = result.current.workflow.edges[0];
    act(() => result.current.insertNodeIntoEdge(edge.id));
    const inserted = result.current.selectedNode!;
    expect(inserted.type).toBe("Script");
    expect(result.current.workflow.edges).toEqual(
      expect.arrayContaining([
        expect.objectContaining({
          fromNodeId: edge.fromNodeId,
          toNodeId: inserted.id,
          condition: edge.condition,
          priority: edge.priority,
        }),
        expect.objectContaining({
          fromNodeId: inserted.id,
          toNodeId: edge.toNodeId,
        }),
      ])
    );
    act(() => result.current.deleteNode(inserted.id, true));
    expect(result.current.workflow.edges).toHaveLength(1);
    expect(result.current.workflow.edges[0]).toMatchObject({
      fromNodeId: edge.fromNodeId,
      toNodeId: edge.toNodeId,
      condition: edge.condition,
      priority: edge.priority,
    });
    expect(result.current.selectedNodeId).toBeNull();
  });
  it("rejects ambiguous reconnect without deleting the node", () => {
    const { result } = designer();
    const start = result.current.workflow.startNodeId;
    act(() => result.current.addNode("Script", 100, 100));
    const script = result.current.selectedNodeId!;
    act(() => result.current.addEdge(start, script));
    const alert = vi.spyOn(window, "alert").mockImplementation(() => {});
    const before = result.current.workflow;
    act(() => result.current.deleteNode(start, true));
    expect(alert).toHaveBeenCalledTimes(1);
    expect(result.current.workflow).toEqual(before);
  });
  it("rejects self-loops and duplicate edges", () => {
    const { result } = designer();
    const edge = result.current.workflow.edges[0];
    act(() => {
      result.current.addEdge(edge.fromNodeId, edge.fromNodeId);
      result.current.addEdge(edge.fromNodeId, edge.toNodeId, edge.condition);
    });
    expect(result.current.workflow.edges).toHaveLength(1);
  });
  it("snaps movement to the grid and undoes a complete drag as one change", () => {
    const { result } = designer();
    const node = result.current.workflow.nodes[0];
    act(() => result.current.pushHistory());
    act(() =>
      result.current.moveNode(node.id, GRID_SIZE + 1, GRID_SIZE * 2 + 1)
    );
    expect(result.current.workflow.nodes[0]).toMatchObject({
      positionX: GRID_SIZE,
      positionY: GRID_SIZE * 2,
    });
    act(() => result.current.undo());
    expect(result.current.workflow.nodes[0]).toEqual(node);
    act(() => result.current.toggleSnap());
    act(() => result.current.moveNode(node.id, 101, 203));
    expect(result.current.workflow.nodes[0]).toMatchObject({
      positionX: 101,
      positionY: 203,
    });
  });
  it("resets dirty state, history and selection when saved data replaces the draft", () => {
    const { result } = designer();
    act(() => result.current.addNode("Script", 100, 100));
    const saved = {
      ...result.current.workflow,
      name: "Saved",
      status: "Published" as const,
    };
    act(() => result.current.replaceSaved(saved));
    expect(result.current.workflow).toEqual(saved);
    expect(result.current.dirty).toBe(false);
    expect(result.current.canUndo).toBe(false);
    expect(result.current.canRedo).toBe(false);
    expect(result.current.selectedNodeId).toBeNull();
    expect(result.current.readOnly).toBe(true);
  });
  it("lays out overlapping nodes without marking a newly opened graph dirty", () => {
    const initial = createWorkflow();
    initial.nodes.forEach((node) => {
      node.positionX = 0;
      node.positionY = 0;
    });
    const { result } = designer(initial);
    const positions = result.current.workflow.nodes.map(
      (node) => `${node.positionX},${node.positionY}`
    );
    expect(new Set(positions).size).toBe(initial.nodes.length);
    expect(result.current.dirty).toBe(false);
  });
});

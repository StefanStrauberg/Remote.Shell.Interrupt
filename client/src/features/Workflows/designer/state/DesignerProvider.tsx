import { PropsWithChildren, useCallback, useReducer, useState } from "react";
import {
  WorkflowDefinition,
  WorkflowNodeType,
} from "../../domain/workflow/model";
import { GRID_SIZE } from "../../domain/workflow/defaults";
import { newNode, layoutWorkflow } from "../../domain/workflow/graph";
import { DesignerContext, DesignerState } from "./designerContext";
type State = {
  graph: WorkflowDefinition;
  saved: string;
  undo: WorkflowDefinition[];
  redo: WorkflowDefinition[];
};
type Action =
  | { type: "edit" | "move"; change: (graph: WorkflowDefinition) => void }
  | { type: "undo" | "redo" | "history" }
  | { type: "saved"; graph: WorkflowDefinition };
function reducer(state: State, action: Action): State {
  if (action.type === "saved")
    return {
      graph: action.graph,
      saved: JSON.stringify(action.graph),
      undo: [],
      redo: [],
    };
  if (state.graph.status !== "Draft") return state;
  if (action.type === "undo") {
    const graph = state.undo[state.undo.length - 1];
    return graph
      ? {
          ...state,
          graph,
          undo: state.undo.slice(0, -1),
          redo: [...state.redo, state.graph],
        }
      : state;
  }
  if (action.type === "redo") {
    const graph = state.redo[state.redo.length - 1];
    return graph
      ? {
          ...state,
          graph,
          undo: [...state.undo, state.graph],
          redo: state.redo.slice(0, -1),
        }
      : state;
  }
  if (action.type === "history")
    return {
      ...state,
      undo: [...state.undo.slice(-99), state.graph],
      redo: [],
    };
  if (action.type === "edit" || action.type === "move") {
    const graph = structuredClone(state.graph);
    action.change(graph);
    return {
      ...state,
      graph,
      undo:
        action.type === "edit"
          ? [...state.undo.slice(-99), state.graph]
          : state.undo,
      redo: [],
    };
  }
  return state;
}
export function DesignerProvider({
  initial,
  children,
}: PropsWithChildren<{ initial: WorkflowDefinition }>) {
  const [state, dispatch] = useReducer(reducer, initial, (graph) => {
    const collapsed =
      graph.nodes.length > 1 &&
      new Set(graph.nodes.map((n) => `${n.positionX},${n.positionY}`)).size ===
        1;
    const arranged = collapsed ? layoutWorkflow(graph) : graph;
    return {
      graph: arranged,
      // Baseline is the *displayed* graph, so an auto-layout applied only for
      // viewing (no designer coordinates yet) doesn't show as unsaved on open.
      saved: JSON.stringify(arranged),
      undo: [],
      redo: [],
    };
  });
  const [busy, setBusy] = useState(false);
  const [snapEnabled, setSnap] = useState(true);
  const [selectedNodeId, selectNodeId] = useState<string | null>(null);
  const [selectedEdgeId, selectEdgeId] = useState<string | null>(null);
  const readOnly = busy || state.graph.status !== "Draft";
  const edit = (change: (graph: WorkflowDefinition) => void) => {
    if (!readOnly) dispatch({ type: "edit", change });
  };
  const moveNode = useCallback(
    (id: string, x: number, y: number) => {
      if (busy) return;
      dispatch({
        type: "move",
        change: (graph) => {
          const node = graph.nodes.find((n) => n.id === id);
          if (node) {
            node.positionX = snapEnabled
              ? Math.round(x / GRID_SIZE) * GRID_SIZE
              : x;
            node.positionY = snapEnabled
              ? Math.round(y / GRID_SIZE) * GRID_SIZE
              : y;
          }
        },
      });
    },
    [busy, snapEnabled]
  );
  const pushHistory = useCallback(() => {
    if (!busy) dispatch({ type: "history" });
  }, [busy]);
  const selectNode = (id: string | null) => {
    selectNodeId(id);
    selectEdgeId(null);
  };
  const clearSelection = () => {
    selectNodeId(null);
    selectEdgeId(null);
  };
  const insert = (id: string, direction: "before" | "after" | "edge") => {
    const graph = state.graph;
    const edge =
      direction === "edge"
        ? graph.edges.find((e) => e.id === id)
        : graph.edges.find((e) =>
            direction === "before" ? e.toNodeId === id : e.fromNodeId === id
          );
    const target = graph.nodes.find(
      (n) => n.id === (direction === "edge" ? edge?.fromNodeId : id)
    );
    if (!target) return;
    const node = newNode(
      "Script",
      target.positionX + (direction === "before" ? -280 : 280),
      target.positionY
    );
    const firstId = crypto.randomUUID();
    const secondId = crypto.randomUUID();
    edit((draft) => {
      draft.nodes.push(node);
      if (edge) {
        draft.edges = draft.edges.filter((e) => e.id !== edge.id);
        draft.edges.push(
          { ...edge, id: firstId, toNodeId: node.id },
          {
            id: secondId,
            fromNodeId: node.id,
            toNodeId: edge.toNodeId,
            condition: null,
            priority: 1,
          }
        );
      } else {
        draft.edges.push({
          id: firstId,
          fromNodeId: direction === "before" ? node.id : target.id,
          toNodeId: direction === "before" ? target.id : node.id,
          condition: null,
          priority: 1,
        });
      }
    });
    selectNode(node.id);
  };
  const value: DesignerState = {
    workflow: state.graph,
    dirty: JSON.stringify(state.graph) !== state.saved,
    readOnly,
    busy,
    setBusy,
    canUndo: state.undo.length > 0 && !readOnly,
    canRedo: state.redo.length > 0 && !readOnly,
    snapEnabled,
    selectedNodeId,
    selectedEdgeId,
    selectedNode:
      state.graph.nodes.find((n) => n.id === selectedNodeId) ?? null,
    selectedEdge:
      state.graph.edges.find((e) => e.id === selectedEdgeId) ?? null,
    selectNode,
    selectEdge: (id) => {
      selectEdgeId(id);
      selectNodeId(null);
    },
    clearSelection,
    pushHistory,
    moveNode,
    updateWorkflow: (patch) => edit((g) => Object.assign(g, patch)),
    replaceSaved: (graph) => {
      dispatch({ type: "saved", graph });
      clearSelection();
    },
    addNode: (type: WorkflowNodeType, x, y) => {
      const node = newNode(
        type,
        snapEnabled ? Math.round(x / GRID_SIZE) * GRID_SIZE : x,
        snapEnabled ? Math.round(y / GRID_SIZE) * GRID_SIZE : y
      );
      edit((g) => {
        if (type === "Start") {
          g.nodes.forEach((n) => {
            if (n.type === "Start") {
              n.type = "Join";
              n.config = {};
            }
          });
          g.startNodeId = node.id;
        }
        g.nodes.push(node);
      });
      selectNode(node.id);
    },
    updateNode: (id, patch) =>
      edit((g) => {
        const node = g.nodes.find((n) => n.id === id);
        if (!node) return;
        if (patch.type === "Start") {
          g.nodes.forEach((n) => {
            if (n.id !== id && n.type === "Start") {
              n.type = "Join";
              n.config = {};
            }
          });
          g.startNodeId = id;
        } else if (patch.type && g.startNodeId === id) g.startNodeId = "";
        Object.assign(node, patch);
      }),
    deleteNode: (id, reconnect = false) => {
      const incoming = state.graph.edges.filter((e) => e.toNodeId === id);
      const outgoing = state.graph.edges.filter((e) => e.fromNodeId === id);
      if (reconnect && (incoming.length > 1 || outgoing.length > 1)) {
        window.alert(
          "Reconnect requires at most one incoming and one outgoing edge."
        );
        return;
      }
      const edgeId = crypto.randomUUID();
      edit((g) => {
        g.nodes = g.nodes.filter((n) => n.id !== id);
        g.edges = g.edges.filter(
          (e) => e.fromNodeId !== id && e.toNodeId !== id
        );
        if (
          reconnect &&
          incoming.length === 1 &&
          outgoing.length === 1 &&
          incoming[0].fromNodeId !== outgoing[0].toNodeId
        )
          g.edges.push({
            ...incoming[0],
            id: edgeId,
            toNodeId: outgoing[0].toNodeId,
          });
        if (g.startNodeId === id) g.startNodeId = "";
      });
      clearSelection();
    },
    duplicateNode: (id) => {
      const source = state.graph.nodes.find((n) => n.id === id);
      if (!source) return;
      const node = structuredClone(source);
      node.id = crypto.randomUUID();
      node.key = `${node.key ?? "node"}-${node.id.slice(0, 8)}`;
      node.name += " copy";
      node.positionX += 48;
      node.positionY += 48;
      if (node.type === "Start") {
        node.type = "Join";
        node.config = {};
      }
      edit((g) => {
        g.nodes.push(node);
      });
      selectNode(node.id);
    },
    setStartNode: (id) =>
      edit((g) => {
        g.startNodeId = id;
        g.nodes.forEach((n) => {
          if (n.id === id) {
            n.type = "Start";
            n.config = {};
          } else if (n.type === "Start") {
            n.type = "Join";
            n.config = {};
          }
        });
      }),
    addEdge: (from, to, condition = null, priority = 1) => {
      const id = crypto.randomUUID();
      edit((g) => {
        if (
          from !== to &&
          !g.edges.some(
            (e) =>
              e.fromNodeId === from &&
              e.toNodeId === to &&
              e.condition === condition
          )
        )
          g.edges.push({
            id,
            fromNodeId: from,
            toNodeId: to,
            condition,
            priority,
          });
      });
    },
    updateEdge: (id, patch) =>
      edit((g) => {
        const edge = g.edges.find((e) => e.id === id);
        if (edge) Object.assign(edge, patch);
      }),
    deleteEdge: (id) => {
      edit((g) => {
        g.edges = g.edges.filter((e) => e.id !== id);
      });
      clearSelection();
    },
    insertNodeIntoEdge: (id) => insert(id, "edge"),
    insertNodeBefore: (id) => insert(id, "before"),
    insertNodeAfter: (id) => insert(id, "after"),
    undo: () => {
      if (!readOnly) dispatch({ type: "undo" });
    },
    redo: () => {
      if (!readOnly) dispatch({ type: "redo" });
    },
    toggleSnap: () => setSnap((s) => !s),
    autoLayout: () => edit((g) => Object.assign(g, layoutWorkflow(g))),
  };
  return (
    <DesignerContext.Provider value={value}>
      {children}
    </DesignerContext.Provider>
  );
}

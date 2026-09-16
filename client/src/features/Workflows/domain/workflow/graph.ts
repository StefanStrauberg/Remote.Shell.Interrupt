import {
  nodeTypes,
  WorkflowDefinition,
  WorkflowNode,
  WorkflowNodeType,
} from "./model";
import { defaultConfigForType } from "./defaults";

export function newNode(
  type: WorkflowNodeType,
  x: number,
  y: number
): WorkflowNode {
  const id = crypto.randomUUID();
  return {
    id,
    type,
    name: type,
    key: `${type.toLowerCase()}-${id.slice(0, 8)}`,
    config: defaultConfigForType(type),
    positionX: x,
    positionY: y,
  };
}

/** Imports the prototype's string IDs and assigns fresh GUIDs, including all references.
 * Copies must not reuse node/edge primary keys belonging to a persisted graph. */
export function importAsDraft(value: unknown): WorkflowDefinition {
  if (!value || typeof value !== "object")
    throw new Error("Invalid workflow JSON.");
  const graph = structuredClone(value) as WorkflowDefinition;
  if (
    typeof graph.name !== "string" ||
    !Array.isArray(graph.nodes) ||
    !Array.isArray(graph.edges)
  )
    throw new Error("Expected a workflow with name, nodes and edges.");
  const ids = new Map<string, string>();
  graph.nodes.forEach((node) => {
    if (
      typeof node.id !== "string" ||
      ids.has(node.id) ||
      !nodeTypes.includes(node.type) ||
      typeof node.name !== "string" ||
      (node.key != null && typeof node.key !== "string") ||
      !node.config ||
      typeof node.config !== "object" ||
      Array.isArray(node.config) ||
      !Number.isFinite(node.positionX) ||
      !Number.isFinite(node.positionY)
    )
      throw new Error("Invalid or duplicate workflow node.");
    ids.set(node.id, crypto.randomUUID());
  });
  if (!ids.has(graph.startNodeId))
    throw new Error("Start node does not exist.");
  graph.edges.forEach((edge) => {
    if (
      !ids.has(edge.fromNodeId) ||
      !ids.has(edge.toNodeId) ||
      !Number.isInteger(edge.priority) ||
      (edge.condition != null && typeof edge.condition !== "string")
    )
      throw new Error("Invalid workflow edge.");
    edge.id = crypto.randomUUID();
    edge.fromNodeId = ids.get(edge.fromNodeId)!;
    edge.toNodeId = ids.get(edge.toNodeId)!;
    edge.condition ??= null;
  });
  graph.startNodeId = ids.get(graph.startNodeId)!;
  graph.nodes.forEach((node) => {
    node.id = ids.get(node.id)!;
    node.key ??= null;
    const pathKeys =
      node.type === "Script"
        ? ["input", "output"]
        : node.type === "Decision"
          ? ["variable"]
          : node.type === "SetVariable"
            ? ["name"]
            : ["output"];
    for (const key of pathKeys) {
      if (typeof node.config[key] === "string")
        node.config[key] = node.config[key].replace(/^\$/, "");
    }
    // Prototype-only SNMP simulation settings are not backend parameters.
    if (node.type === "SnmpGet" || node.type === "SnmpWalk") {
      delete node.config.mockValue;
      delete node.config.mockData;
      delete node.config.timeoutMs;
    }
  });
  graph.id = crypto.randomUUID();
  graph.status = "Draft";
  graph.version =
    Number.isInteger(graph.version) && graph.version > 0 ? graph.version : 1;
  return graph;
}

/** Seeded graphs may have no designer coordinates. Arrange each reachable layer,
 * then place disconnected nodes in a final column; cycles cannot stall the walk. */
export function layoutWorkflow(value: WorkflowDefinition): WorkflowDefinition {
  const graph = structuredClone(value);
  const layers = new Map<string, number>();
  const queue = [graph.startNodeId];
  layers.set(graph.startNodeId, 0);
  for (let index = 0; index < queue.length; index++) {
    const id = queue[index];
    for (const edge of graph.edges.filter((e) => e.fromNodeId === id)) {
      if (!layers.has(edge.toNodeId)) {
        layers.set(edge.toNodeId, (layers.get(id) ?? 0) + 1);
        queue.push(edge.toNodeId);
      }
    }
  }
  const lastLayer = Math.max(0, ...layers.values()) + 1;
  const rows = new Map<number, number>();
  for (const node of graph.nodes) {
    const layer = layers.get(node.id) ?? lastLayer;
    const row = rows.get(layer) ?? 0;
    rows.set(layer, row + 1);
    node.positionX = 48 + layer * 288;
    node.positionY = 48 + row * 168;
  }
  return graph;
}

/** Maps Script-node id -> its scriptSource, for diffing what a save is about to persist. */
export function scriptSourcesOf(
  graph: WorkflowDefinition
): Record<string, string> {
  const sources: Record<string, string> = {};
  for (const node of graph.nodes) {
    if (node.type === "Script") {
      sources[node.id] = String(node.config.scriptSource ?? "");
    }
  }
  return sources;
}

/** True when any Script node's source differs between the two snapshots
 * (added, changed, or removed) - the signal that a save would actually ship
 * different server-executable code, not just move a box on the canvas. */
export function scriptSourcesDiffer(
  before: Record<string, string>,
  after: Record<string, string>
): boolean {
  const ids = new Set([...Object.keys(before), ...Object.keys(after)]);
  for (const id of ids) {
    if ((before[id] ?? "") !== (after[id] ?? "")) return true;
  }
  return false;
}

export function workflowPayload(graph: WorkflowDefinition) {
  return {
    name: graph.name.trim(),
    version: graph.version,
    startNodeId: graph.startNodeId,
    nodes: graph.nodes,
    edges: graph.edges,
  };
}

export function validateWorkflow(graph: WorkflowDefinition): string[] {
  const errors: string[] = [];
  if (!graph.name.trim()) errors.push("Workflow name is required.");
  if (!Number.isInteger(graph.version) || graph.version < 1)
    errors.push("Version must be a positive integer.");
  if (graph.nodes.filter((n) => n.type === "Start").length !== 1)
    errors.push("Exactly one Start node is required.");
  if (
    !graph.nodes.some((n) => n.id === graph.startNodeId && n.type === "Start")
  )
    errors.push("Select a valid Start node.");
  const ids = new Set(graph.nodes.map((n) => n.id));
  if (ids.size !== graph.nodes.length) errors.push("Node IDs must be unique.");
  if (!graph.nodes.some((n) => n.type === "End"))
    errors.push("An End node is required.");
  for (const edge of graph.edges) {
    if (!ids.has(edge.fromNodeId) || !ids.has(edge.toNodeId))
      errors.push("An edge refers to a missing node.");
    if (!Number.isInteger(edge.priority))
      errors.push("Edge priorities must be integers.");
  }
  for (const node of graph.nodes) {
    const requireText = (key: string) => {
      if (
        typeof node.config[key] !== "string" ||
        !(node.config[key] as string).trim()
      )
        errors.push(`${node.name}: ${key} is required.`);
    };
    if (node.type === "Script") {
      requireText("scriptSource");
      const timeout = Number(node.config.timeoutMs ?? 1500);
      if (!Number.isFinite(timeout) || timeout < 50 || timeout > 30000)
        errors.push(`${node.name}: timeout must be 50–30000 ms.`);
    }
    if (node.type === "SnmpGet" || node.type === "SnmpWalk") {
      requireText("oid");
      requireText("output");
    }
    if (node.type === "SetVariable") requireText("name");
    if (node.type === "Decision") requireText("variable");
  }
  return errors;
}

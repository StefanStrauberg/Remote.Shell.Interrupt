export const nodeTypes = [
  "Start",
  "End",
  "Decision",
  "Join",
  "SetVariable",
  "SnmpGet",
  "SnmpWalk",
  "Script",
  "SaveNetworkDevice",
] as const;
export type WorkflowNodeType = (typeof nodeTypes)[number];
export type WorkflowStatus = "Draft" | "Published" | "Archived";
export interface WorkflowNode {
  id: string;
  type: WorkflowNodeType;
  name: string;
  key: string | null;
  config: Record<string, unknown>;
  positionX: number;
  positionY: number;
}
export interface WorkflowEdge {
  id: string;
  fromNodeId: string;
  toNodeId: string;
  condition: string | null;
  priority: number;
}
export interface WorkflowDefinition {
  id: string;
  name: string;
  version: number;
  status: WorkflowStatus;
  startNodeId: string;
  nodes: WorkflowNode[];
  edges: WorkflowEdge[];
}
export interface WorkflowSummary {
  id: string;
  name: string;
  version: number;
  status: WorkflowStatus;
  nodeCount: number;
  edgeCount: number;
}
export interface ExecutionStep {
  nodeName: string;
  nodeType: string;
  outputs: Record<string, unknown>;
  decision: string | null;
  candidateEdges: { condition: string | null; priority: number }[];
  why: string;
  nextNodeName: string | null;
  logs: string[];
}
export interface ExecutionResult {
  success: boolean;
  error: string | null;
  steps: ExecutionStep[];
  finalVariables: Record<string, unknown>;
}
export interface ExecutionRequest {
  host: string;
  community: string;
  input: Record<string, unknown>;
}
export function parseObject(text: string): Record<string, unknown> {
  const value: unknown = JSON.parse(text);
  if (!value || typeof value !== "object" || Array.isArray(value))
    throw new Error("Enter a JSON object.");
  return value as Record<string, unknown>;
}

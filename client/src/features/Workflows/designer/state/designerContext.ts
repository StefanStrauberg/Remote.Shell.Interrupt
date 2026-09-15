import { createContext, useContext } from "react";
import {
  WorkflowDefinition,
  WorkflowEdge,
  WorkflowNode,
  WorkflowNodeType,
} from "../../domain/workflow/model";
export interface DesignerState {
  workflow: WorkflowDefinition;
  dirty: boolean;
  readOnly: boolean;
  busy: boolean;
  setBusy(value: boolean): void;
  canUndo: boolean;
  canRedo: boolean;
  snapEnabled: boolean;
  selectedNodeId: string | null;
  selectedEdgeId: string | null;
  selectedNode: WorkflowNode | null;
  selectedEdge: WorkflowEdge | null;
  selectNode(id: string | null): void;
  selectEdge(id: string | null): void;
  clearSelection(): void;
  pushHistory(): void;
  updateWorkflow(
    patch: Partial<Pick<WorkflowDefinition, "name" | "version">>
  ): void;
  replaceSaved(graph: WorkflowDefinition): void;
  addNode(type: WorkflowNodeType, x: number, y: number): void;
  updateNode(id: string, patch: Partial<WorkflowNode>): void;
  moveNode(id: string, x: number, y: number): void;
  deleteNode(id: string, reconnect?: boolean): void;
  duplicateNode(id: string): void;
  setStartNode(id: string): void;
  addEdge(
    from: string,
    to: string,
    condition?: string | null,
    priority?: number
  ): void;
  updateEdge(id: string, patch: Partial<WorkflowEdge>): void;
  deleteEdge(id: string): void;
  insertNodeIntoEdge(id: string): void;
  insertNodeBefore(id: string): void;
  insertNodeAfter(id: string): void;
  undo(): void;
  redo(): void;
  toggleSnap(): void;
  autoLayout(): void;
}
export const DesignerContext = createContext<DesignerState | null>(null);
export function useDesigner() {
  const value = useContext(DesignerContext);
  if (!value) throw new Error("Workflow designer provider is missing.");
  return value;
}
